using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OTTER
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {
        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {                
                foreach (Sprite sprite in allSprites)
                {                    
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
          
            this.Izbornik.Hide();
            timer1.Start();
            timer2.Start();
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;            
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);

            timer3.Tick += new EventHandler(timer3_Tick);
            timer3.Interval = 10000; // in miliseconds

            
           
            SetupGame();
        }
        /*public void TimerZaVirus()
        {
            timer3.Tick += new EventHandler(timer3_Tick);
            timer3.Interval = 5000; // in miliseconds
            timer3.Start();
        }*/
        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        /* Game variables */
        GlavniLik glavni;
        
        Predmeti virus,virus1,virus2;

        Predmeti maska;
        Predmeti dezinficijens;
        Predmeti banana,jabuka,mlijeko,kruh,pecivo,pomocni;

        Ljudi mama, starac;
        
        List<Predmeti> listaNamirnica;
        List<Predmeti> odabraneNamirnice;

        Sprite startBotun;
        Sprite pozadinaDucan;
        public Form Izbornik;
        Random r=new Random();
        
        private delegate void IspisEvent();
        private event IspisEvent Ispis;

        private delegate void IspisNamirnicaEvent();
        private event IspisNamirnicaEvent IspisNamirnica;

        private delegate void PobjedaEvent();
        private event PobjedaEvent Pobjeda_kraj;

        private void timer3_Tick(object sender, EventArgs e)
        {
                  
        }

        /* Initialization */


        private void SetupGame()
        {
            //1. setup stage
            SetStageTitle("PMF");
            //setBackgroundColor(Color.WhiteSmoke);            
            setBackgroundPicture("backgrounds\\ulica.jpg");
            //none, tile, stretch, center, zoom
            setPictureLayout("stretch");

            //2. add sprites
            glavni = new GlavniLik("sprites\\djecak.png", 0, 0);
            glavni.SetSize(30);
            Game.AddSprite(glavni);

            virus = new Predmeti("sprites\\corona1.png", GameOptions.RightEdge - 100, 0);
            virus.SetVisible(false);
            virus.SetSize(10);
            Game.AddSprite(virus);

            virus1 = new Predmeti("sprites\\corona1.png", GameOptions.RightEdge - 100, 0);
            virus1.SetVisible(false);
            virus1.SetSize(10);
            Game.AddSprite(virus1);

            virus2 = new Predmeti("sprites\\corona1.png", GameOptions.RightEdge - 100, 0);
            virus2.SetVisible(false);
            virus2.SetSize(10);
            Game.AddSprite(virus2);

            maska = new Predmeti("sprites\\mask.png", GameOptions.RightEdge-110, 0);
            maska.SetSize(10);
            maska.SetVisible(false);
            Game.AddSprite(maska);

            dezinficijens = new Predmeti("sprites\\dezinfekcija.png", GameOptions.RightEdge-90, 0);
            dezinficijens.SetSize(15);
            dezinficijens.SetVisible(false);
            Game.AddSprite(dezinficijens);

            startBotun = new Sprite("sprites\\start.png", 400, 550);
            startBotun.SetSize(50);
            Game.AddSprite(startBotun);
            startBotun.SetVisible(false);

            banana = new Predmeti("sprites\\banana.png", GameOptions.RightEdge-200, 400);
            banana.SetVisible(false);
            banana.Naziv = "banana";
            banana.SetTransparentColor(Color.White);
            Game.AddSprite(banana);
            

            mlijeko = new Predmeti("sprites\\mlijeko.png", GameOptions.RightEdge-200, 0);
            mlijeko.SetVisible(false);
            mlijeko.Naziv = "mlijeko";
            mlijeko.SetSize(80);
            mlijeko.SetTransparentColor(Color.White);
            Game.AddSprite(mlijeko);
           

            jabuka = new Predmeti("sprites\\jabuka.png", GameOptions.RightEdge-200, 100);
            jabuka.SetVisible(false);
            jabuka.Naziv = "jabuka";
            jabuka.SetSize(80);
            jabuka.SetTransparentColor(Color.White);
            Game.AddSprite(jabuka);
       

            kruh = new Predmeti("sprites\\kruh.png", GameOptions.RightEdge-200, 200);
            kruh.SetVisible(false);
            kruh.Naziv = "kruh";
            kruh.SetTransparentColor(Color.White);
            kruh.SetSize(75);
            Game.AddSprite(kruh);
            

            pecivo = new Predmeti("sprites\\pecivo.png", GameOptions.RightEdge-200, 300);
            pecivo.SetVisible(false);
            pecivo.Naziv = "pecivo";
            pecivo.SetTransparentColor(Color.White);
            Game.AddSprite(pecivo);

            mama = new Ljudi("sprites\\mama.png", 20, 0);
            mama.SetVisible(false);
            mama.SetTransparentColor(Color.White);
            mama.SetSize(40);
            Game.AddSprite(mama);

            starac = new Ljudi("sprites\\starac.png", 900, 0);
            starac.SetVisible(false);
            starac.SetSize(60);
            Game.AddSprite(starac);

            listaNamirnica = new List<Predmeti>();//nena
            listaNamirnica.Add(banana);
            listaNamirnica.Add(mlijeko);
            listaNamirnica.Add(kruh);
            listaNamirnica.Add(jabuka);
            listaNamirnica.Add(pecivo);

            pozadinaDucan = new Sprite("backgrounds\\ducanProlaz1.png", 0, 0);
            Game.AddSprite(pozadinaDucan);
            pozadinaDucan.SetVisible(false);

            //događaji
            Ispis += IspisiRezultat;
            IspisNamirnica += IspisZaNamirnice;         
            glavni.KrajIgre += GlavniKraj;
            Pobjeda_kraj += Pobjeda;
            Ispis.Invoke();

            //3. scripts that start
            Game.StartScript(GlavniKretanje);
            Game.StartScript(Maska);
            Game.StartScript(Dezinficijens);
            Game.StartScript(VirusKretanje);
            Game.StartScript(DucanMetoda);
            
        }

        /*Metode*/

        private void GlavniKraj()
        {

            Wait(0.1);
            START = false;
            glavni.SetVisible(false);
            virus.SetVisible(false);
            maska.SetVisible(false);
            dezinficijens.SetVisible(false);
            virus1.SetVisible(false);
            virus2.SetVisible(false);
            jabuka.SetVisible(false);
            banana.SetVisible(false);
            mlijeko.SetVisible(false);
            kruh.SetVisible(false);
            pecivo.SetVisible(false);
            starac.SetVisible(false);
            mama.SetVisible(false);
            Wait(0.1);
            setBackgroundPicture("backgrounds\\kraj1.png");

            
        }
        private void Pobjeda()
        {
            Wait(0.1);
            START = false;
            glavni.SetVisible(false);
            virus.SetVisible(false);
            maska.SetVisible(false);
            dezinficijens.SetVisible(false);
            virus1.SetVisible(false);
            virus2.SetVisible(false);
            jabuka.SetVisible(false);
            banana.SetVisible(false);
            mlijeko.SetVisible(false);
            kruh.SetVisible(false);
            pecivo.SetVisible(false);
            starac.SetVisible(false);
            mama.SetVisible(false);
            Wait(0.1);
            setBackgroundPicture("backgrounds\\pobjeda.jpg");
        }
        private void RemoveSprites()
        {
            //vrati brojač na 0
            BGL.spriteCount = 0;
            //izbriši sve spriteove
            BGL.allSprites.Clear();
            //počisti memoriju
            GC.Collect();
        }
        private void IspisiRezultat()
        {
            ISPIS = "Broj maski: " + glavni.SkupljeneMaske + "/" + GameOptions.misija1Maske.ToString() +
                    "\nBroj dezinficijensa: " + glavni.SkupljeniDezinficijensi + "/" + GameOptions.misija1Dez
                    + "\nBroj života: " + glavni.BrojZivota + "/"+GameOptions.broj_zivota;
        }
        private void IspisZaNamirnice()
        {
            ISPIS ="Broj života: "+glavni.BrojZivota;
        }
       
        private void OdaberiNamirnice()
        {
            odabraneNamirnice = new List<Predmeti>();

            if(odabraneNamirnice.Count!=0)
                odabraneNamirnice.Clear();

            while(odabraneNamirnice.Count<3)
            {
                pomocni = listaNamirnica[r.Next(0, 4)];
                if (!odabraneNamirnice.Contains(pomocni))
                     odabraneNamirnice.Add(pomocni);
                
            }
            odabraneNamirnice[0].X = 230;
            odabraneNamirnice[0].Y = 10;
            odabraneNamirnice[1].X = 700;
            odabraneNamirnice[1].Y = 200;
            odabraneNamirnice[2].X = 650;
            odabraneNamirnice[2].Y = 550;
            foreach (Predmeti hrana in odabraneNamirnice)
            {
                if (hrana.Naziv == "banana")
                    banana.SetVisible(true);
                if (hrana.Naziv == "jabuka")
                    jabuka.SetVisible(true);
                if (hrana.Naziv == "mlijeko")
                    mlijeko.SetVisible(true);
                if (hrana.Naziv == "kruh")
                    kruh.SetVisible(true);
                if (hrana.Naziv == "pecivo")
                    pecivo.SetVisible(true);
            }
        }
               
        /* Scripts */

        private int GlavniKretanje()
        {
            glavni.X = 0;
            glavni.Y = (GameOptions.DownEdge-glavni.Heigth) / 2;
            while(START)
            {
                if (sensing.KeyPressed(Keys.Up))
                    glavni.Y -= glavni.Brzina; //svojstva će se pobrinuti da ne izlazi iz pozornice
                if (sensing.KeyPressed(Keys.Down))
                    glavni.Y += glavni.Brzina;

                if(glavni.TouchingSprite(virus))
                {
                    Wait(0.1);
                    virus.SetVisible(false);
                    virus.X = -100;
                    glavni.BrojZivota--;
                    Ispis.Invoke();
                }
                if(glavni.TouchingSprite(maska))
                {
                    Wait(0.1);
                    maska.Dodir = true;
                    glavni.SkupljeneMaske++;
                    Ispis.Invoke();
                }
                if (glavni.TouchingSprite(dezinficijens))
                {
                    Wait(0.1);
                    dezinficijens.Dodir = true;
                    glavni.SkupljeniDezinficijensi++;
                    Ispis.Invoke();
                }

                Wait(0.01);
            }
            return 0;
        }

        private int VirusKretanje()
        {
            virus.X = GameOptions.RightEdge - 110;
            virus.Y = r.Next(0, GameOptions.DownEdge - 130);
            Wait(0.1);
            virus.SetVisible(true);
            virus.Dodir = false;

            while (!virus.Dodir)
            {

                virus.X -= 20;
                if (virus.TouchingEdge())
                {
                    virus.Y = r.Next(0, GameOptions.DownEdge - 130);
                    Wait(0.1);
                }
               
                Wait(0.075);
            }

            if (glavni.BrojZivota != 0)
                Game.StartScript(VirusKretanje);
            return 0;
        }

        private int Maska()
        {
            maska.X = GameOptions.RightEdge - 110;
            maska.Y = r.Next(0, GameOptions.DownEdge - 120);
            Wait(0.1);
            maska.SetVisible(true);
            maska.Dodir = false;
            while (!maska.Dodir)
            {
                maska.X -= 20;
                if (maska.TouchingEdge())
                {
                    maska.Y = r.Next(0, GameOptions.DownEdge - 120);
                    Wait(0.1);
                }
                Wait(0.075);
            }
            if (glavni.SkupljeneMaske != (GameOptions.misija1Maske - 1))
                Game.StartScript(Maska);
            else
            {
                maska.SetVisible(false);
                maska.X = GameOptions.RightEdge;
                maska.Y = 0;
            }

            return 0;
        }

        private int Dezinficijens()
        {
            dezinficijens.GotoXY(GameOptions.RightEdge - 90, r.Next(0, GameOptions.DownEdge - 150));
            Wait(0.1);
            dezinficijens.SetVisible(true);
            dezinficijens.Dodir = false;
            while (!dezinficijens.Dodir)
            {
                dezinficijens.X -= 15;
                if (dezinficijens.TouchingEdge())
                {
                    dezinficijens.Y = r.Next(0, GameOptions.DownEdge - 150);
                    Wait(0.1);
                }
                Wait(0.075);
            }
            if (glavni.SkupljeniDezinficijensi != (GameOptions.misija1Dez - 1))
                Game.StartScript(Dezinficijens);
            else
            {
                dezinficijens.SetVisible(false);
                dezinficijens.X = GameOptions.RightEdge;
                dezinficijens.Y = 0;
            }

            return 0;
        }

        private int GlavniKretanjeDucan()
        {
            START = true;
            while(START)
            {
                Color boja1 = pozadinaDucan.CurrentCostume.GetPixel(sensing.Mouse.X, sensing.Mouse.Y);
                
                if (boja1.R == 254 && boja1.G == 242 && boja1.B == 0)
                {
                    glavni.GotoXY(sensing.Mouse.X - (glavni.Width / 2), sensing.Mouse.Y - (glavni.Heigth / 2));
                }
                if (glavni.TouchingSprite(virus1))
                {
                    Wait(0.1);
                    virus1.SetVisible(false);
                    virus1.X = -100;
                    glavni.BrojZivota--;
                    IspisNamirnica.Invoke();
                    
                }
                if (glavni.TouchingSprite(virus2))
                {
                    Wait(0.1);
                    virus2.SetVisible(false);
                    virus2.X = -100;
                    glavni.BrojZivota--;
                    IspisNamirnica.Invoke();

                }
                if (glavni.TouchingSprite(banana))
                {
                    Wait(0.1);
                    glavni.BrojNamirnica++;
                    banana.Dodir = true;
                    banana.X = GameOptions.RightEdge;
                    IspisNamirnica.Invoke();
                }
                if (glavni.TouchingSprite(mlijeko))
                {
                    Wait(0.1);
                    glavni.BrojNamirnica++;
                    mlijeko.Dodir = true;
                    mlijeko.X = GameOptions.RightEdge;
                    IspisNamirnica.Invoke();
                }
                if (glavni.TouchingSprite(kruh))
                {
                    Wait(0.1);
                    glavni.BrojNamirnica++;
                    kruh.Dodir = true;
                    kruh.X = GameOptions.RightEdge;
                    IspisNamirnica.Invoke();
                }
                if (glavni.TouchingSprite(pecivo))
                {
                    Wait(0.1);
                    glavni.BrojNamirnica++;
                    pecivo.Dodir = true;
                    pecivo.X = GameOptions.RightEdge;
                    IspisNamirnica.Invoke();
                }
                if (glavni.TouchingSprite(jabuka))
                {
                    Wait(0.1);
                    glavni.BrojNamirnica++;
                    jabuka.Dodir = true;
                    jabuka.X = GameOptions.RightEdge;
                    IspisNamirnica.Invoke();
                }
                if(glavni.TouchingSprite(starac))
                {
                    starac.Y = 0;
                    glavni.BrojZivota--;                    
                    IspisNamirnica.Invoke();
                }
                Wait(0.1);

                if(odabraneNamirnice[0].Dodir && odabraneNamirnice[1].Dodir && odabraneNamirnice[2].Dodir)
                {
                    Pobjeda_kraj.Invoke();
                }
                
            }
            return 0;
        }

        private int Virus1KretanjeDucan()
        {
            virus1.X = GameOptions.RightEdge - 110;
            virus1.Y = 220;
            Wait(0.1);
            virus1.SetVisible(true);
            virus1.Dodir = false;

            while (!virus1.Dodir)
            {

                virus1.X -= 20;
                Wait(0.1);
            }

            if (glavni.BrojZivota != 0)
                Game.StartScript(Virus1KretanjeDucan);
            return 0;
        }

        private int Virus2KretanjeDucan()
        {
            virus2.X = GameOptions.LeftEdge;
            virus2.Y = 550;
            Wait(0.1);
            virus2.SetVisible(true);
            virus2.Dodir = false;

            while (!virus2.Dodir)
            {

                virus2.X -= 20;
                Wait(0.1);
            }

            if (glavni.BrojZivota != 0)
                Game.StartScript(Virus2KretanjeDucan);
            return 0;
        }

        private int LjudiKretanje()
        {
            mama.SetVisible(true);
            starac.SetVisible(true);
            while (true)
            {
                mama.Y += 10;
                starac.Y += 10;
                Wait(0.1);
            }
            return 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void BGL_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.RemoveSprites();
            glavni.SkupljeneMaske = 0;
            glavni.SkupljeniDezinficijensi = 0;
            glavni.BrojNamirnica = 0;
            banana.Dodir = false;
            jabuka.Dodir = false;
            kruh.Dodir = false;
            mlijeko.Dodir = false;
            pecivo.Dodir = false;
            this.Izbornik.Show();        
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }                    
           
        private int DucanMetoda()//nena
        {
            while(START)
            {
                if (glavni.SkupljeneMaske == GameOptions.misija1Maske && glavni.SkupljeniDezinficijensi == GameOptions.misija1Dez)
                {
                    Wait(0.1);
                    START = false;                //kako zaustavit petlju al da se kreće još    
                    ISPIS="";
                    virus.SetVisible(false);//nena
                    setBackgroundPicture("backgrounds\\ducanA.png");
                    setPictureLayout("stretch");
                    startBotun.SetVisible(true);

                    glavni.X = 0;
                    glavni.Y = GameOptions.DownEdge - glavni.Heigth;

                    while (!(startBotun.TouchingMousePoint(sensing.Mouse) && sensing.MouseDown))
                    {

                    }
                    Wait(0.2);

                    setBackgroundPicture("backgrounds\\ducanProlaz2.png");//nena
                    setPictureLayout("stretch");
                    
                    startBotun.SetVisible(false);
                    glavni.SetSize(75);
                    glavni.Y = GameOptions.DownEdge - glavni.Heigth;
                    OdaberiNamirnice();
                    
                    IspisNamirnica.Invoke();
                    Game.StartScript(GlavniKretanjeDucan);
                    Game.StartScript(Virus1KretanjeDucan);
                    Game.StartScript(Virus2KretanjeDucan);
                    Game.StartScript(LjudiKretanje);


                }                   
            }
            return 0;
        }
        /* ------------ GAME CODE END ------------ */


    }
}
