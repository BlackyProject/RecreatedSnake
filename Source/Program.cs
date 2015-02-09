using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
/*
 * Creator: SakuraJassen
 * Version: V1.5.2
 */

namespace RecreatedSnake
{
  class Snake
  {
    public int X;
    public int Y;
    public int life;
    public Snake(int x, int y)
    {
      X = x;
      Y = y;
      life = 0;
    }
  }
  class Snak
  {
    public int X;
    public int Y;
    public Snak(int x, int y)
    {
      X = x;
      Y = y;
    }
  }

  class Program
  {
    const int Default_SCORE = 1;
    const int Default_MAXY = 42;
    const int Default_MAXX = 41;
    const int Default_MINY = 1;
    const int Default_MINX = 0;

    static int SCORE = Default_SCORE;
    static int MAXY = Default_MAXY;
    static int MAXX = Default_MAXX;
    static int MINY = Default_MINY;
    static int MINX = Default_MINX;

    static List<Snake> lSnake = new List<Snake>();

    static Snak vFood = new Snak(20,20);

    static int vLenght = 1;
    static int vSleeping = 0;
    static int vWait = 100;
    static int vMaxOfSet = 1; 

    static long vDeltaTime = 0;

    static bool vMode = false;
    static bool vLose = false;
    static bool vDebug = false;

    static eDirection vDirection = eDirection.Standing;
    static eDirection vLastDirection = eDirection.Standing;

    static Stopwatch vSeasonTime = new Stopwatch();
    static Stopwatch vGameTime = new Stopwatch();

    static Random vRan = new Random();

    static string vFPS = "";

    enum eDirection
    {
      Up,
      Down,
      Left,
      Right,
      Standing
    };

    static void Main(string[] args)
    {
      StartUp();
      while (true)
      {
        lSnake.RemoveAll(g => g.X != 100000);
        lSnake.Add(new Snake(20, 20));
        vLose = false;
        vLenght = 1;
        if (vSeasonTime.ElapsedTicks > 0)
        {
          vSeasonTime.Restart();
        }
        while (true && !vLose)
        {
          vDeltaTime = vGameTime.ElapsedMilliseconds;
          if (vDirection != eDirection.Standing)
          {
            vSeasonTime.Start();
          }
          Draw();
          ControlInput();
          Update();
        }
        Draw();
        Thread.Sleep(1000);
        ConsoleKeyInfo buffercki = new ConsoleKeyInfo();
        while (Console.KeyAvailable)
        {
          buffercki = Console.ReadKey(true);
        }
        Console.Write("\nPress any key...");
        Console.ReadKey();
      }
    }

    static void StartUp()
    {
      Console.CursorVisible = false;
      Console.SetWindowSize(80, 80);
      ReadOption();
      vFPS = Convert.ToString(1000 / vWait);
      vGameTime.Start();
    }

    static void ReadOption()
    {
      if (System.IO.File.Exists(@".\Options.xml"))
      {
        System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(@".\Options.xml");

        string vContents = "";
        while (reader.Read())
        {
          reader.MoveToContent();
          if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.Name != "Options" && reader.Name != "Value")
          {
            vContents += reader.Name + ":";
          }
          if (reader.NodeType == System.Xml.XmlNodeType.Text)
          {
            vContents += reader.Value + "-";
          }
        }
        string[] vBuffer = vContents.Split('-');
        for (int i = 0; i < 6; i++)
        {
          string[] vValue = vBuffer[i].Split(':');
          switch (vValue[0])
          {
            case "SCORE":
              SCORE = Convert.ToInt32(vValue[1]);
              break;
            case "MAXX":
              MAXX = Convert.ToInt32(vValue[1]);
              break;
            case "MAXY":
              MAXY = Convert.ToInt32(vValue[1]);
              break;
            case "MINX":
              MINX = Convert.ToInt32(vValue[1]);
              break;
            case "MINY":
              MINY = Convert.ToInt32(vValue[1]);
              break;
            case "vMode":
              vMode = Convert.ToBoolean(vValue[1]);
              break;
            case "vWait":
              vWait = Convert.ToInt32(vValue[1]);
              break;
            default:
              break;
          }
        }
        reader.Close();
      }
      else
      {
        CreateOptions();
      }
    }

    static void CreateOptions(bool Default = false)
    {
        string[]     Options         = new string[7]{ "SCORE", "MAXY", "MAXX", "MINY", "MINX", "vWait", "vMode" };

        int[] OptionValueInt = new int[6] { SCORE, MAXY, MAXX, MINY, MINX, vWait };
        Boolean[] OptionValueBool = new Boolean[1] { vMode };
        if (Default == true)
        {
          OptionValueInt = new int[6] { Default_SCORE, Default_MAXY, Default_MAXX, Default_MINY, Default_MINX, 100 };
          OptionValueBool = new Boolean[1] { false };
        }
        

        List<string> lValues = new List<string>();

        for (int i = 0; i < OptionValueInt.Length; i++)
        {
          lValues.Add(OptionValueInt[i].ToString());
        }
        for (int i = 0; i < OptionValueBool.Length; i++)
        {
          lValues.Add(OptionValueBool[i].ToString());
        }

        using (XmlWriter writer = XmlWriter.Create(@".\Options.xml"))
        {
          writer.WriteStartDocument();
          writer.WriteStartElement("Options");

          for (int i = 0; i < Options.Length; i++)
          {
            writer.WriteStartElement(Options[i]);

            writer.WriteElementString("Value", lValues[i]);

            writer.WriteEndElement();
          }

          writer.WriteEndElement();
          writer.WriteEndDocument();
          writer.Close();
        }
    }

    static void Draw()
    {
      char vChar = ' ';

      string vScore = (lSnake.Count).ToString().PadLeft(6, '0');
      string vScoreMax = (vLenght).ToString().PadLeft(6, '0');
      string vTime = String.Format("{0:00}:{1:00}:{2:00}", vSeasonTime.Elapsed.Hours, vSeasonTime.Elapsed.Minutes, vSeasonTime.Elapsed.Seconds);
      string vModeString = vMode.ToString().PadRight(5);

      if (!vLose)
      {
        StringBuilder builder = new StringBuilder();
        Snake sanke = new Snake(0, 0);
    
        builder.Append(   "Score: "
                        + vScore
                        + " / "
                        + vScoreMax
                        + " Skip: "
                        + vModeString
                        + " Time: "
                        + vTime
                        + " FPS:" 
                        + vFPS 
                        + "\n");

        for (int y = MINY; y < MAXY; y++)
        {
          for (int x = MINX; x < MAXX; x++)
          {
            vChar = ' ';
            sanke = lSnake.Find(s => s.X == x && s.Y == y);
            if (x == vFood.X && y == vFood.Y)
            {
              vChar = '.';
            }
            else if (sanke != null)
            {
              if (sanke.life == 0)
              {
                vChar = 'O';
              }
              else
              {
                vChar = '█';
              }
            }
            builder.Append(vChar);
          }
          builder.Append("│\n");
        }
        for (int x = 0; x < MAXX; x++)
        {
          builder.Append("─");
        }
        builder.Append("┘");
        Console.Clear();
        Console.Write(builder);
      }
      else
      {
        vSeasonTime.Stop();
        Console.Write(  "\nScore: " 
                      + vScore
                      + " Time: "
                      + vTime
                      + "\nGame Over! :C"); 
      }
    }

    static eDirection GetDir(char Dir)
    {
      switch (Dir)
      {
        case 'w':
          return eDirection.Up;
        case 's':
          return eDirection.Down;
        case 'd':
          return eDirection.Right;
        case 'a':
          return eDirection.Left;
      }
      return eDirection.Standing;
    }

    static void ChangeDic(eDirection Direction)
    {
      if (Direction == eDirection.Down && vDirection != eDirection.Up)
      {
        vDirection = Direction;
        return;
      }
      else if (Direction == eDirection.Left && vDirection != eDirection.Right)
      {
        vDirection = Direction;
        return;
      }
      else if (Direction == eDirection.Up && vDirection != eDirection.Down)
      {
        vDirection = Direction;
        return;
      }
      else if (Direction == eDirection.Right && vDirection != eDirection.Left)
      {
        vDirection = Direction;
        return;
      }
    }

    static void ControlInput()
    {
      ConsoleKeyInfo cki = new ConsoleKeyInfo();
      ConsoleKeyInfo buffercki = new ConsoleKeyInfo();
      if (Console.KeyAvailable)
      {
        cki = Console.ReadKey(true);
      }
      while (Console.KeyAvailable && vMode)
      {
        buffercki = Console.ReadKey(true);
      }
      switch (cki.KeyChar)
      {
        case 'w':
        case 's':
        case 'd':
        case 'a':
          ChangeDic(GetDir(cki.KeyChar));
          break;
        case 'p':
          if (vDirection != eDirection.Standing)
          {
            vLastDirection = vDirection;
            vDirection = eDirection.Standing;
            vSeasonTime.Stop();
          }
          else
          {
            vDirection = vLastDirection;
            vSeasonTime.Start();
          }
          break;
        case ' ':
          vMode = !vMode;
          CreateOptions();
          break;
        case 'o':
          ChangeFPS();
          CreateOptions();
          break;
        case 'i':
          vDebug = !vDebug;
          break;
        case 'k':
          CreateOptions(true);
          Environment.Exit(0);
          break;
      }
    }

    static void Update()
    {
      int x;
      int y;
      if (vDirection != eDirection.Standing)
      {
        Snake Head = lSnake.Find(g => g.life == 0);
        switch (vDirection)
        {
          case eDirection.Up:
            x = 0;
            y = -1;
            break;
          case eDirection.Down:
            x = 0;
            y = +1;
            break;
          case eDirection.Left:
            x = -1;
            y = 0;
            break;
          case eDirection.Right:
            x = +1;
            y = 0;
            break;
          default:
            x = 0;
            y = 0;
            break;
        }

        lSnake.ForEach(delegate(Snake name){ name.life++; });

        lSnake.RemoveAll(g => g.life >= vLenght);
        if (lSnake.Exists(g => g.X == Head.X + x && g.Y == Head.Y + y) && vDirection != eDirection.Standing)
        {
          vLose = true;
        }
        if (Head.X + x > MAXX - 1)
        {
          lSnake.Add(new Snake(MINX, Head.Y + y));
        }
        else if (Head.X + x < MINX)
        {
          lSnake.Add(new Snake(MAXX-1, Head.Y + y));
        }
        else if (Head.Y + y > MAXY - 1)
        {
          lSnake.Add(new Snake(Head.X + x, MINY));
        }
        else if (Head.Y + y < MINY)
        {
          lSnake.Add(new Snake(Head.X + x, MAXY-1));
        }
        else
        {
          lSnake.Add(new Snake(Head.X + x, Head.Y + y));
        }
        if (vFood.X == Head.X + x && vFood.Y == Head.Y + y)
        {
          vLenght += SCORE;
          int vFoodX = vRan.Next(MINX + 5, MAXX - 3);
          int vFoodY = vRan.Next(MINY + 5, MAXY - 3);
          while (lSnake.FindIndex(z => z.X == vFoodX && z.Y == vFoodY) != -1)
          {
            vFoodX = vRan.Next(MINX + 5, MAXX - 3);
            vFoodY = vRan.Next(MINY + 5, MAXY - 3);
          }
          vFood.X = vFoodX;
          vFood.Y = vFoodY;
        }
      }

      vSleeping = Convert.ToInt32(vGameTime.ElapsedMilliseconds - vDeltaTime);
      int CoolDown = vWait - vSleeping;
      if (vDebug)
      {
        Console.WriteLine("\n"
                          + CoolDown
                          + " = " 
                          + vWait 
                          + " - " 
                          + vSleeping
                          + "\n"
                          + vSleeping 
                          + " = " 
                          + vGameTime.ElapsedMilliseconds 
                          + " - " 
                          + vDeltaTime);
      }
      try
      {
        Thread.Sleep(CoolDown);
      }
      catch (Exception e)
      {
        Debug.WriteLine("Welp! Something went wronge here! " + e);
      }
    }

    static void ChangeFPS(string Text = "")
    {
      int output;
      int vWaitOld;

      bool err = true;

      vWaitOld = vWait;

      Console.Clear();
      if (Text != "")
      {
        Console.WriteLine(Text);
      }
      Console.WriteLine("How many FPS:");
      vFPS = Console.ReadLine();
      err = int.TryParse(vFPS, out output);

      while (!err)
      {
        Console.WriteLine("Error!\nHow many FPS:");
        vFPS = Console.ReadLine();
        err = int.TryParse(vFPS, out output);
      }
      
      vWait = 1000 / output;

      if (output > 120)
      {
        vWait = vWaitOld;
        ChangeFPS("Please enter a smaller Number!");
      }
    }
  }
}
