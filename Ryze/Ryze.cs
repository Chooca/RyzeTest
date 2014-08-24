using System.Linq;
using System.Reflection;
using LeagueSharp;
using SharpDX;
using System;
using Color = System.Drawing.Color;
using system.IO;

namespace RyzeNFD
{
  internal class Ryze
  {
    private static bool _enableHarass = true;
    private static bool _enableShowMenu = true;
    private static bool _ultAgainstSingle = true;
    private static bool _enableCombo;
    private static bool _enableLongCombo;
    private static int _switch;

    private static void CastQ(Obj_AI_Base target)
    {
      if (((Obj_AI_Base) ObjectManager.get_Player()).get_Spellbook().CanUseSpell((SpellSlot) 0) != null || !Functions.IsValid(target, 625f))
        return;
      Functions.CastSpell(target, (SpellSlot) 0);
    }

    private static void CastW(Obj_AI_Base target)
    {
      if (((Obj_AI_Base) ObjectManager.get_Player()).get_Spellbook().CanUseSpell((SpellSlot) 1) != null || !Functions.IsValid(target, 600f))
        return;
      Functions.CastSpell(target, (SpellSlot) 1);
    }

    private static void CastE(Obj_AI_Base target)
    {
      if (((Obj_AI_Base) ObjectManager.get_Player()).get_Spellbook().CanUseSpell((SpellSlot) 2) != null || !Functions.IsValid(target, 600f))
        return;
      Functions.CastSpell(target, (SpellSlot) 2);
    }

    private static void CastR()
    {
      if (Functions.CountEnemyHeroes(600f) > 1 && ((Obj_AI_Base) ObjectManager.get_Player()).get_Spellbook().CanUseSpell((SpellSlot) 3) == null)
      {
        ((Obj_AI_Base) ObjectManager.get_Player()).get_Spellbook().CastSpell((SpellSlot) 3);
      }
      else
      {
        if (!Ryze._ultAgainstSingle || ((Obj_AI_Base) ObjectManager.get_Player()).get_Spellbook().CanUseSpell((SpellSlot) 3) != null)
          return;
        ((Obj_AI_Base) ObjectManager.get_Player()).get_Spellbook().CastSpell((SpellSlot) 3);
      }
    }

    private static void Harass()
    {
      if (!Ryze._enableHarass || Ryze._enableCombo || Ryze._enableLongCombo)
        return;
      Obj_AI_Hero objAiHero = Program.Ts.Target;
      if (!Functions.IsValid((Obj_AI_Base) objAiHero, 625f))
        return;
      Ryze.CastQ((Obj_AI_Base) objAiHero);
    }

    private static void LongCombo()
    {
      if (!Ryze._enableLongCombo || Ryze._enableCombo)
        return;
      Obj_AI_Hero objAiHero = Program.Ts.Target;
      ((Obj_AI_Base) ObjectManager.get_Player()).IssueOrder((GameObjectOrder) 2, Game.get_CursorPos());
      if (!Functions.IsValid((Obj_AI_Base) objAiHero, 625f))
        return;
      Ryze.CastQ((Obj_AI_Base) objAiHero);
      Ryze.CastW((Obj_AI_Base) objAiHero);
      Ryze.CastE((Obj_AI_Base) objAiHero);
      Ryze.CastR();
    }

    private static void BurstCombo()
    {
      if (!Ryze._enableCombo || Ryze._enableLongCombo)
        return;
      Obj_AI_Hero objAiHero = Program.Ts.Target;
      ((Obj_AI_Base) ObjectManager.get_Player()).IssueOrder((GameObjectOrder) 2, Game.get_CursorPos());
      float num = ((Obj_AI_Base) ObjectManager.get_Player()).get_PercentCooldownMod() * -100f;
      if (!Functions.IsValid((Obj_AI_Base) objAiHero, 625f))
        return;
      if ((double) num >= 20.0)
      {
        if ((double) num > 20.0 && (double) num < 30.0)
        {
          Ryze.CastQ((Obj_AI_Base) objAiHero);
          Ryze.CastE((Obj_AI_Base) objAiHero);
          Ryze.CastW((Obj_AI_Base) objAiHero);
          Ryze.CastR();
        }
        else
        {
          if ((double) num <= 30.0)
            return;
          Ryze.CastQ((Obj_AI_Base) objAiHero);
          Ryze.CastR();
          Ryze.CastW((Obj_AI_Base) objAiHero);
          Ryze.CastE((Obj_AI_Base) objAiHero);
        }
      }
      else
      {
        Ryze.CastQ((Obj_AI_Base) objAiHero);
        Ryze.CastW((Obj_AI_Base) objAiHero);
        Ryze.CastE((Obj_AI_Base) objAiHero);
        Ryze.CastR();
      }
    }

    private static void ShowMenu(string title)
    {
      if (!Ryze._enableShowMenu)
        return;
      Point point = new Point(50, 50);
      int x = point.X;
      int y = point.Y;
      Functions.DrawBox(x - 12, y - 6, 240, 23, 2, (Color) Color.Black, (Color) Color.Red);
      Functions.DrawBox(x - 12, y + 20, 240, 78, 2, (Color) Color.Black, (Color) Color.Red);
      Functions.DrawGameTextPixel(x - 12 + 90, y - 2, (Color) Color.White, title);
      Functions.DrawGameTextPixel(x, y + 30, (Color) Color.White, "Combo (Space):");
      Functions.DrawGameTextPixel(x, y + 45, (Color) Color.White, "Long Combo (X):");
      Functions.DrawGameTextPixel(x, y + 60, (Color) Color.White, "Auto Harass (T):");
      Functions.DrawGameTextPixel(x, y + 75, (Color) Color.White, "Ult Single Target (G):");
      if (Ryze._enableCombo)
        Functions.DrawGameTextPixel(x + 163, y + 30, (Color) Color.Lime, "Enabled");
      else
        Functions.DrawGameTextPixel(x + 158, y + 30, (Color) Color.Red, "Disabled");
      if (Ryze._enableLongCombo)
        Functions.DrawGameTextPixel(x + 163, y + 45, (Color) Color.Lime, "Enabled");
      else
        Functions.DrawGameTextPixel(x + 158, y + 45, (Color) Color.Red, "Disabled");
      if (Ryze._enableHarass)
        Functions.DrawGameTextPixel(x + 163, y + 60, (Color) Color.Lime, "Enabled");
      else
        Functions.DrawGameTextPixel(x + 158, y + 60, (Color) Color.Red, "Disabled");
      if (Ryze._ultAgainstSingle)
        Functions.DrawGameTextPixel(x + 163, y + 75, (Color) Color.Lime, "Enabled");
      else
        Functions.DrawGameTextPixel(x + 158, y + 75, (Color) Color.Red, "Disabled");
    }

    public static void Game_OnGameUpdate(EventArgs args)
    {
      Ryze.BurstCombo();
      Ryze.LongCombo();
      Ryze.Harass();
    }

    public static void Game_OnWndProc(WndEventArgs args)
    {
      if ((int) args.get_Msg() == 256)
      {
        uint wparam = args.get_WParam();
        if (wparam <= 45U)
        {
          if ((int) wparam != 32)
          {
            if ((int) wparam != 45 || Environment.TickCount - Ryze._switch <= 85)
              return;
            Ryze._enableShowMenu = !Ryze._enableShowMenu;
            Ryze._switch = Environment.TickCount;
          }
          else
            Ryze._enableCombo = true;
        }
        else if ((int) wparam != 71)
        {
          if ((int) wparam != 84)
          {
            if ((int) wparam != 88)
              return;
            Ryze._enableLongCombo = true;
          }
          else
          {
            if (Environment.TickCount - Ryze._switch <= 85)
              return;
            Ryze._enableHarass = !Ryze._enableHarass;
            Ryze._switch = Environment.TickCount;
          }
        }
        else
        {
          if (Environment.TickCount - Ryze._switch <= 85)
            return;
          Ryze._ultAgainstSingle = !Ryze._ultAgainstSingle;
          Ryze._switch = Environment.TickCount;
        }
      }
      else
      {
        if ((int) args.get_Msg() != 257)
          return;
        switch (args.get_WParam())
        {
          case 32U:
            Ryze._enableCombo = false;
            break;
          case 88U:
            Ryze._enableLongCombo = false;
            break;
        }
      }
    }

    public static void Drawing_OnDraw(EventArgs args)
    {
      Ryze.ShowMenu("Information");
      if (!((GameObject) ObjectManager.get_Player()).get_IsDead())
        Drawing.DrawCircle(((GameObject) ObjectManager.get_Player()).get_Position(), 625f, Color.Red);
      Drawing.DrawCircle(((GameObject) Program.Ts.Target).get_Position(), 50f, Color.Red);
    }
  }
}
