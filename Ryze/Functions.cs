//
using LeagueSharp;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace RyzeNFD
{
  internal class Functions
  {
    private static string GetActiveWindowTitle()
    {
      StringBuilder text = new StringBuilder(256);
      return Functions.GetWindowText(Functions.GetForegroundWindow(), text, 256) <= 0 ? (string) null : ((object) text).ToString();
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    public static bool IsLoLinForeground()
    {
      return Functions.GetActiveWindowTitle() != null && Functions.GetActiveWindowTitle().Contains("League of Legends");
    }

    public static float GetDistance3D(Obj_AI_Base from, Obj_AI_Base to)
    {
      return Vector3.Distance(from.get_ServerPosition(), to.get_ServerPosition());
    }

    public static bool IsValid(Obj_AI_Base target)
    {
      if (((GameObject) target).get_IsValid() && ((AttackableUnit) target).get_IsTargetable() && (((GameObject) target).get_IsVisible() && !((GameObject) target).get_IsDead()))
        return !((AttackableUnit) target).get_IsInvulnerable();
      else
        return false;
    }

    public static bool IsValid(Obj_AI_Base target, float range)
    {
      if (((GameObject) target).get_IsValid() && ((AttackableUnit) target).get_IsTargetable() && (((GameObject) target).get_IsVisible() && !((GameObject) target).get_IsDead()) && !((AttackableUnit) target).get_IsInvulnerable())
        return (double) Functions.GetDistance3D(target, (Obj_AI_Base) ObjectManager.get_Player()) < (double) range;
      else
        return false;
    }

    public static int CountEnemyHeroes(float range)
    {
      int num = 0;
      using (IEnumerator<Obj_AI_Hero> enumerator = ((IEnumerable<Obj_AI_Hero>) ObjectManager.Get<Obj_AI_Hero>()).GetEnumerator())
      {
        if (((IEnumerator) enumerator).MoveNext())
        {
          Obj_AI_Hero current = enumerator.Current;
          if (Functions.IsValid((Obj_AI_Base) current) && ((GameObject) current).get_Team() != ((GameObject) ObjectManager.get_Player()).get_Team())
            return num + 1;
          else
            return 0;
        }
      }
      return 0;
    }

    public static void CastSpell(Obj_AI_Base target, SpellSlot slot)
    {
      MemoryStream memoryStream = new MemoryStream();
      BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream);
      binaryWriter.Write((byte) 153);
      binaryWriter.Write(((GameObject) ObjectManager.get_Player()).get_NetworkId());
      binaryWriter.Write((byte) ((uint) (byte) slot & 63U));
      binaryWriter.Write((float) Game.get_CursorPos().X);
      binaryWriter.Write((float) Game.get_CursorPos().Y);
      binaryWriter.Write((float) Game.get_CursorPos().X);
      binaryWriter.Write((float) Game.get_CursorPos().Y);
      binaryWriter.Write(((GameObject) target).get_NetworkId());
      Game.SendPacket(memoryStream.ToArray(), (PacketChannel) 1, (PacketProtocolFlags) 0);
    }

    public static void UseItem(int id, Obj_AI_Hero target)
    {
      foreach (InventorySlot inventorySlot in ((Obj_AI_Base) ObjectManager.get_Player()).get_InventoryItems())
      {
        if (inventorySlot.get_Id() == id)
        {
          try
          {
            inventorySlot.UseItem((GameObject) target);
          }
          catch
          {
          }
        }
      }
    }

    public static void FillRgb(int x, int y, int w, int h, Color color)
    {
      if (w < 0)
        w = 1;
      if (h < 0)
        h = 1;
      if (x < 0)
        x = 1;
      if (y < 0)
        y = 1;
      Rectangle rectangle;
      // ISSUE: explicit reference operation
      ((Rectangle) @rectangle).\u002Ector(x, y, w, h);
      Rectangle[] rectangleArray = new Rectangle[1]
      {
        rectangle
      };
      Drawing.get_Direct3DDevice().Clear((ClearFlags) 1, ColorBGRA.op_Implicit(color), 0.0f, 0, rectangleArray);
    }

    public static void DrawCircle(int x, int y, Color circleColor)
    {
      Functions.FillRgb(x - 1, y - 1, 2, 2, circleColor);
    }

    public static void DrawBorder(int x, int y, int w, int h, int px, Color bordercolor)
    {
      Functions.FillRgb(x, y + h - px, w, px, bordercolor);
      Functions.FillRgb(x, y, px, h, bordercolor);
      Functions.FillRgb(x, y, w, px, bordercolor);
      Functions.FillRgb(x + w - px, y, px, h, bordercolor);
    }

    public static void DrawBox(int x, int y, int w, int h, int di, Color boxColor, Color borderColor)
    {
      Functions.FillRgb(x, y, w, h, boxColor);
      Functions.DrawBorder(x, y, w, h, di, borderColor);
    }

    public static void DrawGameTextPixel(int x, int y, Color textcolor, string text)
    {
      if (string.IsNullOrEmpty(text))
        return;
      Program.CreatedFont.DrawText((Sprite) null, text, x, y, ColorBGRA.op_Implicit(textcolor));
    }
  }
}
