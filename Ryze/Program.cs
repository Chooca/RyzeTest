//This is ZaraX script i only edited it.

using LeagueSharp;
using SharpDX;
using SharpDX.Direct3D9;
using System;

namespace Le_SBTW
{
  internal class Program
  {
    public static Font CreatedFont = (Font) null;
    public static SimpleTs Ts;

    private static void Main(string[] args)
    {
      GameMode gameMode = (GameMode) 1;
      while (gameMode != 2)
        gameMode = Game.get_Mode();
      if (((Obj_AI_Base) ObjectManager.get_Player()).get_BaseSkinName() != "Ryze" && ((Obj_AI_Base) ObjectManager.get_Player()).get_BaseSkinName() != "Akali")
        return;
      Program.Initialize();
    }

    private static void Initialize()
    {
      // ISSUE: method pointer
      Game.add_OnGameStart(new GameStart((object) null, __methodptr(Game_OnGameStart)));
      // ISSUE: method pointer
      Drawing.add_OnEndScene(new EndScene((object) null, __methodptr(Drawing_OnEndScene)));
      // ISSUE: method pointer
      Drawing.add_OnPostReset(new PostReset((object) null, __methodptr(Drawing_OnPostReset)));
      // ISSUE: method pointer
      Drawing.add_OnPreReset(new PreReset((object) null, __methodptr(Drawing_OnPreReset)));
      switch (((Obj_AI_Base) ObjectManager.get_Player()).get_BaseSkinName())
      {
        case "Ryze":
          Program.Ts = new SimpleTs(625f, SimpleTs.TargetingMode.AutoPriority);
          // ISSUE: method pointer
          Game.add_OnGameUpdate(new GameUpdate((object) null, __methodptr(Game_OnGameUpdate)));
          // ISSUE: method pointer
          Game.add_OnWndProc(new WndProc((object) null, __methodptr(Game_OnWndProc)));
          // ISSUE: method pointer
          Drawing.add_OnDraw(new Draw((object) null, __methodptr(Drawing_OnDraw)));
          break;
        case "Akali":
          Program.Ts = new SimpleTs(800f, SimpleTs.TargetingMode.AutoPriority);
          // ISSUE: method pointer
          Game.add_OnGameUpdate(new GameUpdate((object) null, __methodptr(Game_OnGameUpdate)));
          // ISSUE: method pointer
          Game.add_OnWndProc(new WndProc((object) null, __methodptr(Game_OnWndProc)));
          // ISSUE: method pointer
          Drawing.add_OnDraw(new Draw((object) null, __methodptr(Drawing_OnDraw)));
          // ISSUE: method pointer
          Obj_AI_Base.add_OnProcessSpellCast(new GameObjectProcessSpellCast((object) null, __methodptr(Obj_AI_Hero_OnProcessSpellCast)));
          break;
      }
    }

    private static void Game_OnGameStart(EventArgs args)
    {
      Game.PrintChat("<font color='00FF00'>>> Good Luck & Have Fun ! <<</font>");
    }

    private static void Drawing_OnEndScene(EventArgs args)
    {
      if (Program.CreatedFont != null)
        return;
      Device direct3Ddevice = Drawing.get_Direct3DDevice();
      FontDescription fontDescription1 = (FontDescription) null;
      fontDescription1.Height = (__Null) 15;
      fontDescription1.FaceName = (__Null) "Arial";
      fontDescription1.Italic = (__Null) Bool.op_Implicit(false);
      fontDescription1.Width = (__Null) 0;
      fontDescription1.MipLevels = (__Null) 1;
      fontDescription1.CharacterSet = (__Null) 1;
      fontDescription1.OutputPrecision = (__Null) 0;
      fontDescription1.Quality = (__Null) 4;
      fontDescription1.PitchAndFamily = (__Null) 0;
      fontDescription1.Weight = (__Null) 600;
      FontDescription fontDescription2 = fontDescription1;
      Program.CreatedFont = new Font(direct3Ddevice, fontDescription2);
    }

    private static void Drawing_OnPreReset(EventArgs args)
    {
      if (Program.CreatedFont == null)
        return;
      Program.CreatedFont.OnLostDevice();
    }

    private static void Drawing_OnPostReset(EventArgs args)
    {
      if (Program.CreatedFont == null)
        return;
      Program.CreatedFont.OnResetDevice();
    }
  }
}
