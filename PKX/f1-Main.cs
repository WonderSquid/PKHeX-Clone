﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;

namespace PKHeX
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            #region Pop open a splash screen while we load up.
            Thread SplashSCR = new Thread(() => new SplashScreen(this).ShowDialog());
            SplashSCR.Start();
            #endregion
            #region Initialize Form
            InitializeComponent();
            // Get Default White Color (for some users who have different color schemes
            CB_ExtraBytes.SelectedIndex = 0;

            // Resize Main Window to PKX Editing Mode
            largeWidth = this.Width;
            shortWidth = (Width * (30500 / 620)) / 100 + 1;
            Width = shortWidth;
            #endregion
            #region Language Detection before loading
            // Set up Language Selection
            string[] main_langlist = new string[]
            {
                "English", // ENG
                "日本語", // JPN
                "Français", // FRE
                "Italiano", // ITA
                "Deutsch", // GER
                "Español", // SPA
                "한국어", // KOR
                "中文", // CHN
            };
            int[] main_langnum = new int[]
            { 2,1,3,4,5,7,8,9 };
            string[] lang_val = { "en", "ja", "fr", "it", "de", "es", "ko", "zh" };
            
            for (int i = 0; i < main_langlist.Length; i++)
            {
                cbItem item = new cbItem();
                item.Text = main_langlist[i];
                item.Value = main_langnum[i];

                CB_MainLanguage.Items.Add(item);
            }
            // Try and detect the language
            string filename = Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            string lastTwoChars = filename.Substring(filename.Length - 2);
            int lang = Array.IndexOf(main_langnum,Array.IndexOf(lang_val, lastTwoChars));

            if (lang >= 0)
                CB_MainLanguage.SelectedIndex = main_langnum[lang];
            else if (lastTwoChars == "jp")
                CB_MainLanguage.SelectedIndex = 1;
            else
                CB_MainLanguage.SelectedIndex = 0;

            #region HaX
            HaX = (filename.IndexOf("HaX") >= 0);
            {
                CHK_HackedStats.Enabled = CHK_HackedStats.Visible =
                DEV_Ability.Enabled = DEV_Ability.Visible =
                MT_Level.Enabled = MT_Level.Visible =
                TB_AbilityNumber.Visible =
                MT_Form.Enabled = MT_Form.Visible = HaX; 

                TB_Level.Visible =
                CB_Ability.Visible = !HaX; 
            }
            #endregion
            #endregion
            #region Localize & Populate
            InitializeStrings();
            // Initialize Fields
            InitializeFields();
            #endregion
            #region Add ContextMenus to the PictureBoxes (PKX slots)
            PictureBox[] pba = {
                                   bpkx1, bpkx2, bpkx3, bpkx4, bpkx5, bpkx6,
                                   bpkx7, bpkx8, bpkx9, bpkx10,bpkx11,bpkx12,
                                   bpkx13,bpkx14,bpkx15,bpkx16,bpkx17,bpkx18,
                                   bpkx19,bpkx20,bpkx21,bpkx22,bpkx23,bpkx24,
                                   bpkx25,bpkx26,bpkx27,bpkx28,bpkx29,bpkx30,

                                    ppkx1, ppkx2, ppkx3, ppkx4, ppkx5, ppkx6,
                               };
            ContextMenuStrip mnu = new ContextMenuStrip();
            ToolStripMenuItem mnuView = new ToolStripMenuItem("View");
            ToolStripMenuItem mnuSet = new ToolStripMenuItem("Set");
            ToolStripMenuItem mnuDelete = new ToolStripMenuItem("Delete");
            //Assign event handlers
            mnuView.Click += new EventHandler(rcmView_Click);
            mnuSet.Click += new EventHandler(rcmSet_Click);
            mnuDelete.Click += new EventHandler(rcmDelete_Click);
            //Add to main context menu
            mnu.Items.AddRange(new ToolStripItem[] { mnuView, mnuSet, mnuDelete });
            //Assign to datagridview
            for (int i = 0; i < pba.Length; i++)
                pba[i].ContextMenuStrip = mnu;

            // Add ContextMenus to the PictureBoxes that are read only
            PictureBox[] pba2 = {
                                    bbpkx1,bbpkx2,bbpkx3,bbpkx4,bbpkx5,bbpkx6,

                                    dcpkx1, dcpkx2, gtspkx, fusedpkx,subepkx1,subepkx2,subepkx3,
                               };
            ContextMenuStrip mnu2 = new ContextMenuStrip();
            ToolStripMenuItem mnu2View = new ToolStripMenuItem("View");
            //Assign event handlers
            mnu2View.Click += new EventHandler(rcmView_Click);
            //Add to main context menu
            mnu2.Items.AddRange(new ToolStripItem[] { mnu2View });
            //Assign to datagridview
            for (int i = 0; i < pba2.Length; i++)
                pba2[i].ContextMenuStrip = mnu2;
            #endregion
            #region Enable Drag and Drop on the form & tab control.
            this.tabMain.AllowDrop = true;
            this.DragEnter += new DragEventHandler(tabMain_DragEnter);
            this.DragDrop += new DragEventHandler(tabMain_DragDrop);

            // Enable Drag and Drop on each tab.
            this.tabMain.DragEnter += new DragEventHandler(tabMain_DragEnter);
            this.tabMain.DragDrop += new DragEventHandler(tabMain_DragDrop);

            TabPage[] tca = { Tab_Main, Tab_Met, Tab_Stats, Tab_Attacks, Tab_OTMisc };
            for (int i = 0; i < tca.Length; i++)
            {
                tca[i].DragEnter += new DragEventHandler(tabMain_DragEnter);
                tca[i].DragDrop += new DragEventHandler(tabMain_DragDrop);
            } 

            // Export D&D
            dragout.MouseDown += new MouseEventHandler(dragout_MouseDown);
            dragout.DragOver += new DragEventHandler(dragout_DragOver);
            eragout.MouseDown += new MouseEventHandler(dragout_MouseDown);
            eragout.DragOver += new DragEventHandler(dragout_DragOver);

            // ToolTips for Drag&Drop
            ToolTip dragoutTip1 = new ToolTip();
            ToolTip dragoutTip2 = new ToolTip();
            dragoutTip1.SetToolTip(dragout, "PK6 QuickSave");
            dragoutTip2.SetToolTip(eragout, "EK6 QuickSave");

            // Box Drag & Drop
            for (int i = 0; i < 30; i++)
                pba[i].AllowDrop = true;
            tabMain.AllowDrop = true;

            #endregion
            #region Finish Up

            // Default Selected Items
            C_BoxSelect.SelectedIndex = 0;
            CB_PPu1.SelectedIndex = CB_PPu2.SelectedIndex = CB_PPu3.SelectedIndex = CB_PPu4.SelectedIndex = 0;
            CB_Ball.SelectedIndex = 0;

            // Load the arguments
            string[] args = Environment.GetCommandLineArgs();
            SDFLoc = Util.GetSDFLocation();
            if (args.Length > 1)
                openQuick(args[1]);
            else if (SDFLoc != null)
                openQuick(Path.Combine(SDFLoc,"main"));
            else if (File.Exists(Util.NormalizePath(Path.Combine(Util.GetTempFolder(), "root" + Path.DirectorySeparatorChar  + "main"))))
                openQuick(Util.NormalizePath(Path.Combine(Util.GetTempFolder() , "root" + Path.DirectorySeparatorChar  + "main")));

            // Close splash screen.  
            init = true;          
            SplashSCR.Join();
            this.BringToFront();
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
            if (HaX)
                Util.Alert("Illegal mode activated.", "Please behave.");
            #endregion
        }

        #region Global Variables: Always Visible!
        public byte[] buff = new Byte[260];
        public byte[] savefile = new Byte[0x100000];
        public byte[] cyberSAV = new Byte[0x65600];
        public bool savegame_oras = false;
        public bool cybergadget = false;
        public bool savLoaded = false;
        public int gt = 258;
        public int genderflag, species;
        public int savindex;
        public bool savedited;
        public string SDFLoc = null;
        public bool HaX = false;

        public static int colorizedbox = 32;
        public static Image colorizedcolor = null;
        public static int colorizedslot = 0;
        public static int largeWidth, shortWidth = 0;
        public static string eggname = "";
        public static string[] gendersymbols = { "♂", "♀", "-" };
        public static string[] specieslist = { };
        public static string[] movelist = { };
        public static string[] itemlist = { };
        public static string[] abilitylist = { };
        public static string[] types = { };
        public static string[] natures = { };
        public static string[] characteristics = { };
        public static string[] memories = { };
        public static string[] genloc = { };
        public static string[] forms = { };
        public static string[] metHGSS_00000 = { };
        public static string[] metHGSS_02000 = { };
        public static string[] metHGSS_03000 = { };
        public static string[] metBW2_00000 = { };
        public static string[] metBW2_30000 = { };
        public static string[] metBW2_40000 = { };
        public static string[] metBW2_60000 = { };
        public static string[] metXY_00000 = { };
        public static string[] metXY_30000 = { };
        public static string[] metXY_40000 = { };
        public static string[] metXY_60000 = { };
        public static string[] trainingbags = { };
        public static string[] trainingstage = { };
        public static string[] wallpapernames = { };
        public static string[] puffs = { };
        public static string[] itempouch = { };
        public static int[] saveoffsets = { };
        public static string origintrack;
        public static string curlanguage;
        public volatile bool init = false;
        public ToolTip Tip1 = new ToolTip();
        public ToolTip Tip2 = new ToolTip();
        public ToolTip Tip3 = new ToolTip();
        public SaveGames.SaveStruct SaveGame = new SaveGames.SaveStruct("XY");
        #endregion
        
        #region //// PKX WINDOW FUNCTIONS ////
        private void InitializeFields()
        {
            // Initialize Fields
            {
                #region var tables
                
                var language_list = new[] {
                    new { Text = "ENG (English)", Value = 2 },
                    new { Text = "JPN (日本語)", Value = 1 },
                    new { Text = "FRE (Français)", Value = 3 },
                    new { Text = "ITA (Italiano)", Value = 4 },
                    new { Text = "GER (Deutsch)", Value = 5 },
                    new { Text = "ESP (Español)", Value = 7 },
                    new { Text = "KOR (한국어)", Value = 8 }
                };
                
                var dsregion_list = new[] {
                    new { Text = "Americas (NA/SA)", Value = 1 },
                    new { Text = "Europe (EU)", Value = 2 },
                    new { Text = "Japan (日本)", Value = 0 },
                    new { Text = "China (中国)", Value = 4 },
                    new { Text = "Korea (한국)", Value = 5 },
                    new { Text = "Taiwan (臺灣)", Value = 6 }
                };
                var subreg_list = new[] {
                    new { Text = "sr_0", Value = 0 },
                    new { Text = "sr_1", Value = 1 },
                    new { Text = "sr_2", Value = 2 },
                    new { Text = "sr_3", Value = 3 },
                    new { Text = "sr_4", Value = 4 },
                    new { Text = "sr_5", Value = 5 },
                    new { Text = "sr_6", Value = 6 },
                    new { Text = "sr_7", Value = 7 },
                    new { Text = "sr_8", Value = 8 },
                    new { Text = "sr_9", Value = 9 },
                    new { Text = "sr_10", Value = 10 },
                    new { Text = "sr_11", Value = 11 },
                    new { Text = "sr_12", Value = 12 },
                    new { Text = "sr_13", Value = 13 },
                    new { Text = "sr_14", Value = 14 },
                    new { Text = "sr_15", Value = 15 },
                    new { Text = "sr_16", Value = 16 },
                    new { Text = "sr_17", Value = 17 },
                    new { Text = "sr_18", Value = 18 },
                    new { Text = "sr_19", Value = 19 },
                    new { Text = "sr_20", Value = 20 },
                    new { Text = "sr_21", Value = 21 },
                    new { Text = "sr_22", Value = 22 },
                    new { Text = "sr_23", Value = 23 },
                    new { Text = "sr_24", Value = 24 },
                    new { Text = "sr_25", Value = 25 },
                    new { Text = "sr_26", Value = 26 },
                    new { Text = "sr_27", Value = 27 },
                    new { Text = "sr_28", Value = 28 },
                    new { Text = "sr_29", Value = 29 },
                    new { Text = "sr_30", Value = 30 },
                    new { Text = "sr_31", Value = 31 },
                    new { Text = "sr_32", Value = 32 },
                    new { Text = "sr_33", Value = 33 },
                    new { Text = "sr_34", Value = 34 },
                    new { Text = "sr_35", Value = 35 },
                    new { Text = "sr_36", Value = 36 },
                    new { Text = "sr_37", Value = 37 },
                    new { Text = "sr_38", Value = 38 },
                    new { Text = "sr_39", Value = 39 },
                    new { Text = "sr_40", Value = 40 },
                    new { Text = "sr_41", Value = 41 },
                    new { Text = "sr_42", Value = 42 },
                    new { Text = "sr_43", Value = 43 },
                    new { Text = "sr_44", Value = 44 },
                    new { Text = "sr_45", Value = 45 },
                    new { Text = "sr_46", Value = 46 },
                    new { Text = "sr_47", Value = 47 },
                    new { Text = "sr_48", Value = 48 },
                    new { Text = "sr_49", Value = 49 },
                    new { Text = "sr_50", Value = 50 },
                    new { Text = "sr_51", Value = 51 },
                    new { Text = "sr_52", Value = 52 },
                    new { Text = "sr_53", Value = 53 },
                    new { Text = "sr_54", Value = 54 },
                    new { Text = "sr_55", Value = 55 },
                    new { Text = "sr_56", Value = 56 },
                    new { Text = "sr_57", Value = 57 },
                    new { Text = "sr_58", Value = 58 },
                    new { Text = "sr_59", Value = 59 },
                    new { Text = "sr_60", Value = 60 },
                    new { Text = "sr_61", Value = 61 },
                    new { Text = "sr_62", Value = 62 },
                    new { Text = "sr_63", Value = 63 },
                    new { Text = "sr_64", Value = 64 },
                    new { Text = "sr_65", Value = 65 },
                    new { Text = "sr_66", Value = 66 },
                    new { Text = "sr_67", Value = 67 },
                    new { Text = "sr_68", Value = 68 },
                    new { Text = "sr_69", Value = 69 },
                    new { Text = "sr_70", Value = 70 },
                    new { Text = "sr_71", Value = 71 },
                    new { Text = "sr_72", Value = 72 },
                    new { Text = "sr_73", Value = 73 },
                    new { Text = "sr_74", Value = 74 },
                    new { Text = "sr_75", Value = 75 },
                    new { Text = "sr_76", Value = 76 },
                    new { Text = "sr_77", Value = 77 },
                    new { Text = "sr_78", Value = 78 },
                    new { Text = "sr_79", Value = 79 },
                    new { Text = "sr_80", Value = 80 },
                };
                #endregion

                // Set ComboBox Fields
                
                CB_3DSReg.DataSource = dsregion_list;
                CB_3DSReg.DisplayMember = "Text";
                CB_3DSReg.ValueMember = "Value";

                CB_Language.DataSource = language_list;
                CB_Language.DisplayMember = "Text";
                CB_Language.ValueMember = "Value";

                setCountry(CB_Country);

                CB_SubRegion.DataSource = subreg_list;
                CB_SubRegion.DisplayMember = "Text";
                CB_SubRegion.ValueMember = "Value";
                
                InitializeLanguage();
            }

            // Finish setting up the ComboBoxes
            CB_Ball.SelectedIndex = 0;
            CB_GameOrigin.SelectedIndex = 0;

            // Now that the ComboBoxes are ready, load the data.
            populateFields(buff);
            {
                TB_OT.Text = "PKHeX";
                TB_TID.Text = 12345.ToString();
                TB_SID.Text = 54321.ToString();
            }
            updateAbilityList(TB_AbilityNumber,Util.getIndex(CB_Species),CB_Ability,CB_Form);
        }
        private void InitializeLanguage()
        {
            #region Balls
            {
                // Allowed Balls
                int[] ball_nums = { 7, 576, 13, 492, 497, 14, 495, 493, 496, 494, 11, 498, 8, 6, 12, 15, 9, 5, 499, 10, 1, 16 };
                int[] ball_vals = { 7, 25, 13, 17, 22, 14, 20, 18, 21, 19, 11, 23, 8, 6, 12, 15, 9, 5, 24, 10, 1, 16 };

                // Set up
                List<cbItem> ball_list = new List<cbItem>();

                for (int i = 4; i > 1; i--) // add 4,3,2
                {
                    // First 3 Balls are always first
                    cbItem ncbi = new cbItem();
                    ncbi.Text = itemlist[i];
                    ncbi.Value = i;
                    ball_list.Add(ncbi);
                }

                // Sort the Rest based on String Name
                string[] ballnames = new string[ball_nums.Length];
                for (int i = 0; i < ball_nums.Length; i++)
                    ballnames[i] = itemlist[ball_nums[i]];

                string[] sortedballs = new string[ball_nums.Length];
                Array.Copy(ballnames, sortedballs, ballnames.Length);
                Array.Sort(sortedballs);

                // Add the rest of the balls
                for (int i = 0; i < sortedballs.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sortedballs[i];
                    ncbi.Value = ball_vals[Array.IndexOf(ballnames, sortedballs[i])];
                    ball_list.Add(ncbi);
                }
                CB_Ball.DisplayMember = "Text";
                CB_Ball.ValueMember = "Value";
                CB_Ball.DataSource = ball_list;
            }
            #endregion
            #region Held Items
            {
                // List of valid items to hold
                int[] item_nums = { 
                                            000,001,002,003,004,005,006,007,008,009,010,011,012,013,014,015,017,018,019,020,021,022,023,024,025,026,027,028,029,030,031,032,033,034,035,
                                            036,037,038,039,040,041,042,043,044,045,046,047,048,049,050,051,052,053,054,055,056,057,058,059,060,061,062,063,064,065,066,067,068,069,070,
                                            071,072,073,074,075,076,077,078,079,080,081,082,083,084,085,086,087,088,089,090,091,092,093,094,099,100,101,102,103,104,105,106,107,108,109,
                                            110,112,116,117,118,119,134,135,136,149,150,151,152,153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,174,
                                            175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,
                                            210,211,212,213,214,215,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,237,238,239,240,241,242,243,244,
                                            245,246,247,248,249,250,251,252,253,254,255,256,257,258,259,260,261,262,263,264,265,266,267,268,269,270,271,272,273,274,275,276,277,278,279,
                                            280,281,282,283,284,285,286,287,288,289,290,291,292,293,294,295,296,297,298,299,300,301,302,303,304,305,306,307,308,309,310,311,312,313,314,
                                            315,316,317,318,319,320,321,322,323,324,325,326,327,504,537,538,539,540,541,542,543,544,545,546,547,548,549,550,551,552,553,554,555,556,557,
                                            558,559,560,561,562,563,564,565,566,567,568,569,570,571,572,573,577,580,581,582,583,584,585,586,587,588,589,590,591,639,640,644,645,646,647,
                                            648,649,650,652,653,654,655,656,657,658,659,660,661,662,663,664,665,666,667,668,669,670,671,672,673,674,675,676,677,678,679,680,681,682,683,
                                            684,685,686,687,688,699,704,708,709,710,711,715,

                                            // Appended ORAS Items (Orbs & Mega Stones)
                                            534,535,
                                            752,753,754,755,756,757,758,759,760,761,762,763,764,767,768,769,770,
                                    };
                if (DEV_Ability.Enabled)    // allow all items to be selected
                {
                    item_nums = new int[itemlist.Length];
                    for (int i = 0; i < itemlist.Length; i++)
                        item_nums[i] = i;
                }

                List<cbItem> item_list = new List<cbItem>();
                // Sort the Rest based on String Name
                string[] itemnames = new string[item_nums.Length];
                for (int i = 0; i < item_nums.Length; i++)
                    itemnames[i] = itemlist[item_nums[i]];

                string[] sorteditems = new string[item_nums.Length];
                Array.Copy(itemnames, sorteditems, itemnames.Length);
                Array.Sort(sorteditems);

                // Add the rest of the items
                for (int i = 0; i < sorteditems.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sorteditems[i];
                    ncbi.Value = item_nums[Array.IndexOf(itemnames, sorteditems[i])];
                    item_list.Add(ncbi);
                }
                CB_HeldItem.DisplayMember = "Text";
                CB_HeldItem.ValueMember = "Value";
                CB_HeldItem.DataSource = item_list;
            }
            #endregion
            #region Species
            {
                List<cbItem> species_list = new List<cbItem>();
                // Sort the Rest based on String Name
                string[] sortedspecies = new string[specieslist.Length];
                Array.Copy(specieslist, sortedspecies, specieslist.Length);
                Array.Sort(sortedspecies);

                // Add the rest of the items
                for (int i = 0; i < sortedspecies.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sortedspecies[i];
                    ncbi.Value = Array.IndexOf(specieslist, sortedspecies[i]);
                    species_list.Add(ncbi);
                }
                CB_Species.DisplayMember = "Text";
                CB_Species.ValueMember = "Value";
                CB_Species.DataSource = species_list;
            }
            #endregion
            #region HAX Ability
            {
                List<cbItem> ability_list = new List<cbItem>();
                // Sort the Rest based on String Name
                string[] sortedability = new string[abilitylist.Length];
                Array.Copy(abilitylist, sortedability, abilitylist.Length);
                Array.Sort(sortedability);

                // Add the rest of the items
                for (int i = 0; i < sortedability.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sortedability[i];
                    ncbi.Value = Array.IndexOf(abilitylist, sortedability[i]);
                    ability_list.Add(ncbi);
                }
                DEV_Ability.DisplayMember = "Text";
                DEV_Ability.ValueMember = "Value";
                DEV_Ability.DataSource = ability_list;
            }
            #endregion
            #region Natures
            {
                List<cbItem> natures_list = new List<cbItem>();
                // Sort the Rest based on String Name
                string[] sortednatures = new string[natures.Length];
                Array.Copy(natures, sortednatures, natures.Length);
                Array.Sort(sortednatures);

                // Add the rest of the items
                for (int i = 0; i < sortednatures.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sortednatures[i];
                    ncbi.Value = Array.IndexOf(natures, sortednatures[i]);
                    natures_list.Add(ncbi);
                }
                CB_Nature.DisplayMember = "Text";
                CB_Nature.ValueMember = "Value";
                CB_Nature.DataSource = natures_list;
            }
            #endregion
            #region Moves
            {
                List<cbItem> move_list = new List<cbItem>();
                // Sort the Rest based on String Name
                string[] sortedmoves = new string[movelist.Length];
                Array.Copy(movelist, sortedmoves, movelist.Length);
                Array.Sort(sortedmoves);

                // Add the rest of the items
                for (int i = 0; i < sortedmoves.Length; i++)
                {
                    cbItem ncbi = new cbItem();
                    ncbi.Text = sortedmoves[i];
                    ncbi.Value = Array.IndexOf(movelist, sortedmoves[i]);
                    move_list.Add(ncbi);
                }

                CB_Move1.DisplayMember = CB_Move2.DisplayMember = CB_Move3.DisplayMember = CB_Move4.DisplayMember = "Text";
                CB_RelearnMove1.DisplayMember = CB_RelearnMove2.DisplayMember = CB_RelearnMove3.DisplayMember = CB_RelearnMove4.DisplayMember = "Text";
                CB_Move1.ValueMember = CB_Move2.ValueMember = CB_Move3.ValueMember = CB_Move4.ValueMember = "Value";
                CB_RelearnMove1.ValueMember = CB_RelearnMove2.ValueMember = CB_RelearnMove3.ValueMember = CB_RelearnMove4.ValueMember = "Value";

                var move1_list = new BindingSource(move_list, null);
                CB_Move1.DataSource = move1_list;

                var move2_list = new BindingSource(move_list, null);
                CB_Move2.DataSource = move2_list;

                var move3_list = new BindingSource(move_list, null);
                CB_Move3.DataSource = move3_list;

                var move4_list = new BindingSource(move_list, null);
                CB_Move4.DataSource = move4_list;

                var eggmove1_list = new BindingSource(move_list, null);
                CB_RelearnMove1.DataSource = eggmove1_list;

                var eggmove2_list = new BindingSource(move_list, null);
                CB_RelearnMove2.DataSource = eggmove2_list;

                var eggmove3_list = new BindingSource(move_list, null);
                CB_RelearnMove3.DataSource = eggmove3_list;

                var eggmove4_list = new BindingSource(move_list, null);
                CB_RelearnMove4.DataSource = eggmove4_list;
            }
            #endregion
            #region Encounter Types

            var EncounterType = new[] {
                    new { Text = "None", Value = 0 },
                    new { Text = "Tall Grass", Value = 2 },
                    new { Text = "Dialga/Palkia", Value = 4 },
                    new { Text = "Cave/Hall of Origin", Value = 5 },
                    new { Text = "Surfing/Fishing", Value = 7 },
                    new { Text = "Building", Value = 9 },
                    new { Text = "Marsh/Safari", Value = 10 },
                    new { Text = "Starter/Fossil/Gift", Value = 24 }
                };
            CB_EncounterType.DataSource = EncounterType;
            CB_EncounterType.DisplayMember = "Text";
            CB_EncounterType.ValueMember = "Value";
            #endregion
            #region Games
            List<cbItem> origin_list = new List<cbItem>();
            // lazy text table... 8 columns
            string[] langlistorigin = new string[] {
                // ID       // EN           // JP               // FR               // IT           // DE               // ES               // KO           // ZH
                    "27",   "OR",           "OR",               "OR",               "OR",           "OR",               "OR",               "OR",          "OR",            
                    "26",   "AS",           "AS",               "AS",               "AS",           "AS",               "AS",               "AS",          "AS",            
                    "24",	"X",	        "X",	            "X",	            "X",	        "X",	            "X",	            "X",           "X",	         
                    "25",	"Y",	        "Y",	            "Y",	            "Y",	        "Y",	            "Y",	            "Y",           "Y",	         
                    "20",	"White",	    "ホワイト",	        "Blanche",	        "Bianca",	    "Weiße",	        "Blanca",	        "화이트",      "ホワイト",	     
                    "21",	"Black",	    "ブラック",	        "Noire",	        "Nera",	        "Schwarze",	        "Negra",	        "블랙",        "ブラック",	     
                    "22",	"White2",	    "ホワイト2",	        "Blanche 2",	    "Bianca2",	    "Weiße2",	        "Blanca2",	        "화이트2",     "ホワイト2",	     
                    "23",	"Black2",	    "ブラック2",	        "Noire 2",	        "Nera2",	    "Schwarze2",        "Negra2",	        "블랙2",       "ブラック2",	     
                    "10",	"Diamond",	    "ダイヤモンド",	    "Diamant",	        "Diamante", 	"Diamant",	        "Diamante",	        "디아루가",     "ダイヤモンド",	 
                    "11",	"Pearl",	    "パール",	        "Perle",	        "Perla",	    "Perl",	            "Perla",	        "펄기아",      "パール",	     
                    "12",	"Platinum",	    "プラチナ",	        "Platine",	        "Platino",	    "Platin",	        "Platino",	        "Pt 기라티나",  "プラチナ",	     
                    "7",	"HeartGold",    "ハートゴールド",	"Or HeartGold",     "HeartGold",	"HeartGold",        "HeartGold",	    "하트골드",     "ハートゴールド",
                    "8",	"SoulSilver",   "ソウルシルバー ",   "Argent SoulSilver","SoulSilver",	"SoulSilver",       "SoulSilver",	    "소울실버",     "ソウルシルバー ",
                    "2",	"Ruby",	        "ルビー",	        "Rubis",            "Rubino",	    "Rubin",	        "Rubí",	            "루비",        "ルビー",	     
                    "1",	"Sapphire",	    "サファイア",	    "Saphir",	        "Zaffiro",	    "Saphir",	        "Zafiro",	        "사파이어",     "サファイア",	 
                    "3",	"Emerald",	    "エメラルド",	    "Émeraude",	        "Smeraldo",	    "Smaragd",	        "Esmeralda",	    "에메랄드",     "エメラルド",	 
                    "4",	"FireRed",	    "ファイアレッド",	"Rouge Feu",        "Rosso Fuoco",	"Feuerrote ",       "Rojo Fuego",	    "파이어레드",   "ファイアレッド",
                    "5",	"LeafGreen",	"リーフグリーン",    "Vert Feuille",     "Verde Foglia", "Blattgrüne ",      "Verde Hoja",	    "리프그린",     "リーフグリーン", 
                    "15",	"Colosseum/XD",	"コロシアム/XD",	    "Colosseum/XD",     "Colosseo/XD",	"Kolosseum/XD",	    "Colosseum/XD",	    "세움/XD",     "コロシアム/XD",
                };
            // populate the list
            for (int i = 0; i < langlistorigin.Length / 9; i++)
            {
                cbItem item = new cbItem();
                item.Text = langlistorigin[i * 9 + CB_MainLanguage.SelectedIndex + 1];
                item.Value = Convert.ToInt32(langlistorigin[i * 9]);
                origin_list.Add(item);
            }

			/*
			 * Moving the assignment of the Display and ValueMemeber to before the DataSource is assigned
			as assigning the DataSource causes a onSelectedIndexChanged which causes a call
			to updateOriginGame which access the SelectedValue property, but since there is no
			ValueMember assigned it uses the cbItem to string method which returns the Text member
			*/
			CB_GameOrigin.DisplayMember = "Text";
			CB_GameOrigin.ValueMember = "Value";
            CB_GameOrigin.DataSource = origin_list;

            #endregion
        }
        private void populateFields(byte[] buff)
        {
            CAL_EggDate.Value = new DateTime(2000, 01, 01);
            Tab_Main.Focus();
            // Encryption Constant
            TB_EC.Text = BitConverter.ToUInt32(buff, 0).ToString("X8");

            // Block A
            int species = BitConverter.ToInt16(buff, 0x08);
            int helditem = BitConverter.ToUInt16(buff, 0x0A);
            uint TID = BitConverter.ToUInt16(buff, 0x0C);
            uint SID = BitConverter.ToUInt16(buff, 0x0E);
            uint exp = BitConverter.ToUInt32(buff, 0x10);
            int ability = buff[0x14];
            int abilitynum = buff[0x15];
            // 0x16, 0x17 - unknown
            uint PID = BitConverter.ToUInt32(buff, 0x18);
            int nature = buff[0x1C];
            int feflag = buff[0x1D] % 2;
            this.genderflag = (buff[0x1D] >> 1) & 0x3;
            int altforms = (buff[0x1D] >> 3);
            int HP_EV = buff[0x1E];
            int ATK_EV = buff[0x1F];
            int DEF_EV = buff[0x20];
            int SPA_EV = buff[0x22];
            int SPD_EV = buff[0x23];
            int SPE_EV = buff[0x21];
            int cnt_cool = buff[0x24];
            int cnt_beauty = buff[0x25];
            int cnt_cute = buff[0x26];
            int cnt_smart = buff[0x27];
            int cnt_tough = buff[0x28];
            int cnt_sheen = buff[0x29];
            int markings = buff[0x2A];
            int PKRS_Strain = buff[0x2B] >> 4;
            int PKRS_Duration = buff[0x2B] % 0x10;

            // Medals and Ribbons, passed with buff to new form
            // 0x2C, 0x2D, 0x2E, 0x2F
            // 0x33, 0x34, 0x35, 0x36
            // 0x34, 0x35, 0x36, 0x37
            // 0x38, 0x39

            // 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F - unused/unknown

            // Block B
            string nicknamestr = Util.TrimFromZero(Encoding.Unicode.GetString(buff, 0x40, 24));
            // 0x58, 0x59 - unused
            int move1 = BitConverter.ToInt16(buff, 0x5A);
            int move2 = BitConverter.ToInt16(buff, 0x5C);
            int move3 = BitConverter.ToInt16(buff, 0x5E);
            int move4 = BitConverter.ToInt16(buff, 0x60);
            int move1_pp = buff[0x62];
            int move2_pp = buff[0x63];
            int move3_pp = buff[0x64];
            int move4_pp = buff[0x65];
            int move1_ppu = buff[0x66];
            int move2_ppu = buff[0x67];
            int move3_ppu = buff[0x68];
            int move4_ppu = buff[0x69];
            int eggmove1 = BitConverter.ToInt16(buff, 0x6A);
            int eggmove2 = BitConverter.ToInt16(buff, 0x6C);
            int eggmove3 = BitConverter.ToInt16(buff, 0x6E);
            int eggmove4 = BitConverter.ToInt16(buff, 0x70);

            // 0x72 - Super Training Flag - Passed with buff to new form

            // 0x73 - unused/unknown
            uint IV32 = BitConverter.ToUInt32(buff, 0x74);
            uint HP_IV = IV32 & 0x1F;
            uint ATK_IV = (IV32 >> 5) & 0x1F;
            uint DEF_IV = (IV32 >> 10) & 0x1F;
            uint SPE_IV = (IV32 >> 15) & 0x1F;
            uint SPA_IV = (IV32 >> 20) & 0x1F;
            uint SPD_IV = (IV32 >> 25) & 0x1F;
            uint isegg = (IV32 >> 30) & 1;
            uint isnick = (IV32 >> 31);

            // Block C
            string notOT = Util.TrimFromZero(Encoding.Unicode.GetString(buff, 0x78, 24));
            bool notOTG = Convert.ToBoolean(buff[0x92]);
            // Memory Editor edits everything else with buff in a new form

            // Block D
            string ot = Util.TrimFromZero(Encoding.Unicode.GetString(buff, 0xB0, 24));
            // 0xC8, 0xC9 - unused
            int OTfriendship = buff[0xCA];
            int OTaffection = buff[0xCB]; // Handled by Memory Editor
            // 0xCC, 0xCD, 0xCE, 0xCF, 0xD0
            int egg_year = buff[0xD1];
            int egg_month = buff[0xD2];
            int egg_day = buff[0xD3];
            int met_year = buff[0xD4];
            int met_month = buff[0xD5];
            int met_day = buff[0xD6];
            // 0xD7 - unused
            int eggloc = BitConverter.ToUInt16(buff, 0xD8);
            int metloc = BitConverter.ToUInt16(buff, 0xDA);
            int ball = buff[0xDC];
            int metlevel = buff[0xDD] & 0x7F;
            int otgender = (buff[0xDD]) >> 7;
            int encountertype = buff[0xDE];
            int gamevers = buff[0xDF];
            int countryID = buff[0xE0];
            int regionID = buff[0xE1];
            int dsregID = buff[0xE2];
            int otlang = buff[0xE3];
            // 0xE4, 0xE5, 0xE6, 0xE7 - unused

            //
            // Populate Fields
            //

            CHK_Fateful.Checked = Convert.ToBoolean(feflag);
            CHK_IsEgg.Checked = Convert.ToBoolean(isegg);
            CHK_Nicknamed.Checked = Convert.ToBoolean(isnick);
            if (Convert.ToBoolean(otgender))
                Label_OTGender.Text = gendersymbols[1];
            else Label_OTGender.Text = gendersymbols[0];
            
            // Nidoran Gender Fixing Text
            if (!Convert.ToBoolean(isnick))
            {
                if (nicknamestr.Contains((char)0xE08F))
                    nicknamestr = Regex.Replace(nicknamestr, "\uE08F", "\u2640");
                else if (nicknamestr.Contains((char)0xE08E))
                    nicknamestr = Regex.Replace(nicknamestr, "\uE08E", "\u2642");
            }
            populateMarkings(markings);
            TB_PID.Text = PID.ToString("X8");
            CB_Species.SelectedValue = species;
            CB_HeldItem.SelectedValue = helditem;
            updateAbilityList(TB_AbilityNumber, species, CB_Ability, CB_Form);
            TB_AbilityNumber.Text = abilitynum.ToString();
            if (abilitynum>>1 < 3) CB_Ability.SelectedIndex = abilitynum>>1; // error handling
            else CB_Ability.SelectedIndex = 0;
            CB_Nature.SelectedValue = nature;

            TB_EXP.Text = exp.ToString();
            TB_TID.Text = TID.ToString();
            TB_SID.Text = SID.ToString();

            TB_OT.Text = ot;
            TB_Nickname.Text = nicknamestr;
            TB_OTt2.Text = notOT;

            if (buff[0x93] == 1)  // = 1
            {
                TB_Friendship.Text = buff[0xA2].ToString();
                GB_nOT.BackColor = System.Drawing.Color.FromArgb(232, 255, 255);
                GB_OT.BackColor = Color.Transparent;

            }
            else                // = 0
            {
                TB_Friendship.Text = OTfriendship.ToString();
                GB_OT.BackColor = System.Drawing.Color.FromArgb(232, 255, 255);
                GB_nOT.BackColor = Color.Transparent;
            }
            
            CB_Language.SelectedValue = otlang;
            CB_Country.SelectedValue = countryID;
            CB_SubRegion.SelectedValue = regionID;
            CB_3DSReg.SelectedValue = dsregID;
            CB_GameOrigin.SelectedValue = gamevers;
            CB_EncounterType.SelectedValue = encountertype;
            CB_Ball.SelectedValue = ball;

            if (met_month == 0)
            { met_month = 1; }
            if (met_day == 0)
            { met_day = 1; }
            try
            { CAL_MetDate.Value = new DateTime(met_year + 2000, met_month, met_day); }
            catch { CAL_MetDate.Value = new DateTime(2000, 1, 1); }

            if (eggloc != 0)
            {
                // Was obtained initially as an egg.
                CHK_AsEgg.Checked = true;
                GB_EggConditions.Enabled = true;

                CB_EggLocation.SelectedValue = eggloc;
                try
                { CAL_EggDate.Value = new DateTime(egg_year + 2000, egg_month, egg_day); }
                catch { CAL_MetDate.Value = new DateTime(2000, 1, 1); }
            }
            else { CHK_AsEgg.Checked = GB_EggConditions.Enabled = false; CB_EggLocation.SelectedValue = 0; }

            CB_MetLocation.SelectedValue = metloc;

            if (notOTG) Label_CTGender.Text = gendersymbols[1];
            else Label_CTGender.Text = gendersymbols[0];
            if (TB_OTt2.Text == "") Label_CTGender.Text = "";

            TB_MetLevel.Text = metlevel.ToString();

            // Reset
            CHK_Cured.Checked = false;
            CHK_Infected.Checked = false;

            CB_PKRSStrain.SelectedIndex = PKRS_Strain;
            CB_PKRSDays.SelectedIndex = Math.Min((PKRS_Duration & 0x7),4); // to strip out bad hacked 'rus
            if (PKRS_Strain > 0)
            {
                CHK_Infected.Checked = true;
                if (PKRS_Duration == 0)
                    CHK_Cured.Checked = true;
            }
            // Do it again now that our comboboxes should be properly set?
            CB_PKRSStrain.SelectedIndex = PKRS_Strain;
            CB_PKRSDays.SelectedIndex = Math.Min((PKRS_Duration & 0x7),4); // to strip out bad hacked 'rus

            TB_Cool.Text = cnt_cool.ToString();
            TB_Beauty.Text = cnt_beauty.ToString();
            TB_Cute.Text = cnt_cute.ToString();
            TB_Smart.Text = cnt_smart.ToString();
            TB_Tough.Text = cnt_tough.ToString();
            TB_Sheen.Text = cnt_sheen.ToString();

            TB_HPIV.Text = HP_IV.ToString();
            TB_ATKIV.Text = ATK_IV.ToString();
            TB_DEFIV.Text = DEF_IV.ToString();
            TB_SPAIV.Text = SPA_IV.ToString();
            TB_SPDIV.Text = SPD_IV.ToString();
            TB_SPEIV.Text = SPE_IV.ToString();

            TB_HPEV.Text = HP_EV.ToString();
            TB_ATKEV.Text = ATK_EV.ToString();
            TB_DEFEV.Text = DEF_EV.ToString();
            TB_SPAEV.Text = SPA_EV.ToString();
            TB_SPDEV.Text = SPD_EV.ToString();
            TB_SPEEV.Text = SPE_EV.ToString();

            CB_Move1.SelectedValue = move1;
            CB_Move2.SelectedValue = move2;
            CB_Move3.SelectedValue = move3;
            CB_Move4.SelectedValue = move4;
            CB_RelearnMove1.SelectedValue = eggmove1;
            CB_RelearnMove2.SelectedValue = eggmove2;
            CB_RelearnMove3.SelectedValue = eggmove3;
            CB_RelearnMove4.SelectedValue = eggmove4;
            CB_PPu1.SelectedIndex = move1_ppu;
            CB_PPu2.SelectedIndex = move2_ppu;
            CB_PPu3.SelectedIndex = move3_ppu;
            CB_PPu4.SelectedIndex = move4_ppu;

            int level;
            if (Util.ToInt32(TB_EXP.Text) == 0) { level = 1; }
            else level = PKX.getLevel(species, ref exp);
            TB_Level.Text = level.ToString();

            // Setup Forms
            setForms(species, CB_Form);
            try
            { CB_Form.SelectedIndex = altforms; }
            catch 
            { 
                if (CB_Form.Items.Count > 1) 
                    CB_Form.SelectedIndex = CB_Form.Items.Count-1; 
                else CB_Form.SelectedIndex = 0;
            }

            // Load Extrabyte Value
            TB_ExtraByte.Text = buff[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();
            if ((TB_OTt2.Text == "") || (notOT == ""))
                Label_CTGender.Text = "";
            
            // Reload Gender Flag
            this.genderflag = ((buff[0x1D] >> 1) & 0x3);
            setGenderLabel();
            updateStats();
            setIsShiny();
            if (init)
                if (!PKX.verifychk(buff))
                    Util.Alert("PKX File has an invalid checksum.");
            
            init = true;

            // DEV Illegality
            DEV_Ability.SelectedValue = ability;
            MT_Level.Text = level.ToString();
            MT_Form.Text = altforms.ToString();
        }
        private void populateMarkings(int markings)
        {
            int m1 = ((markings) & 1);
            int m2 = ((markings >> 1) & 1);
            int m3 = ((markings >> 2) & 1);
            int m4 = ((markings >> 3) & 1);
            int m5 = ((markings >> 4) & 1);
            int m6 = ((markings >> 5) & 1);

            CHK_Circle.Checked = Convert.ToBoolean(m1);
            CHK_Triangle.Checked = Convert.ToBoolean(m2);
            CHK_Square.Checked = Convert.ToBoolean(m3);
            CHK_Heart.Checked = Convert.ToBoolean(m4);
            CHK_Star.Checked = Convert.ToBoolean(m5);
            CHK_Diamond.Checked = Convert.ToBoolean(m6);
        }
        // PKX Data Calculation Functions // 
        private void setIsShiny()
        {
            uint PID = Util.getHEXval(TB_PID);
            uint UID = (PID >> 16);
            uint LID = (PID & 0xFFFF);
            uint PSV = UID ^ LID;
            uint TSV = Util.ToUInt32(TB_TID.Text) ^ Util.ToUInt32(TB_SID.Text);
            uint XOR = TSV ^ PSV;
            int game = Util.getIndex(CB_GameOrigin);
            if (XOR < 16)
            {   // Is Shiny
                BTN_Shinytize.Visible =
                BTN_Shinytize.Enabled = false;
                Label_IsShiny.Visible = true;
            }
            else
            {   // Is Not Shiny
                BTN_Shinytize.Visible =
                BTN_Shinytize.Enabled = true;
                Label_IsShiny.Visible = false;
            }
            setMarkings();
        }
        public void setForms(int species, ComboBox cb)
        {
            // Form Tables
            // 
            var form_unown = new[] {
                    new { Text = "A", Value = 0 },
                    new { Text = "B", Value = 1 },
                    new { Text = "C", Value = 2 },
                    new { Text = "D", Value = 3 },
                    new { Text = "E", Value = 4 },
                    new { Text = "F", Value = 5 },
                    new { Text = "G", Value = 6 },
                    new { Text = "H", Value = 7 },
                    new { Text = "I", Value = 8 },
                    new { Text = "J", Value = 9 },
                    new { Text = "K", Value = 10 },
                    new { Text = "L", Value = 11 },
                    new { Text = "M", Value = 12 },
                    new { Text = "N", Value = 13 },
                    new { Text = "O", Value = 14 },
                    new { Text = "P", Value = 15 },
                    new { Text = "Q", Value = 16 },
                    new { Text = "R", Value = 17 },
                    new { Text = "S", Value = 18 },
                    new { Text = "T", Value = 19 },
                    new { Text = "U", Value = 20 },
                    new { Text = "V", Value = 21 },
                    new { Text = "W", Value = 22 },
                    new { Text = "X", Value = 23 },
                    new { Text = "Y", Value = 24 },
                    new { Text = "Z", Value = 25 },
                    new { Text = "!", Value = 26 },
                    new { Text = "?", Value = 27 },
                };
            var form_castform = new[] {
                    new { Text = types[0], Value = 0 }, // Normal
                    new { Text = forms[789], Value = 1 }, // Sunny
                    new { Text = forms[790], Value = 2 }, // Rainy
                    new { Text = forms[791], Value = 3 }, // Snowy
                };
            var form_shellos = new[] {
                    new { Text = forms[422], Value = 0 }, // West
                    new { Text = forms[811], Value = 1 }, // East
                };
            var form_deoxys = new[] {
                    new { Text = types[0], Value = 0 }, // Normal
                    new { Text = forms[802], Value = 1 }, // Attack
                    new { Text = forms[803], Value = 2 }, // Defense
                    new { Text = forms[804], Value = 3 }, // Speed
                };
            var form_burmy = new[] {
                    new { Text = forms[412], Value = 0 }, // Plant
                    new { Text = forms[805], Value = 1 }, // Sandy
                    new { Text = forms[806], Value = 2 }, // Trash
                };
            var form_cherrim = new[] {
                    new { Text = forms[421], Value = 0 }, // Overcast
                    new { Text = forms[809], Value = 1 }, // Sunshine
                };
            var form_rotom = new[] {
                    new { Text = types[0], Value = 0 }, // Normal
                    new { Text = forms[817], Value = 1 }, // Heat
                    new { Text = forms[818], Value = 2 }, // Wash
                    new { Text = forms[819], Value = 3 }, // Frost
                    new { Text = forms[820], Value = 4 }, // Fan
                    new { Text = forms[821], Value = 5 }, // Mow
                };
            var form_giratina = new[] {
                    new { Text = forms[487], Value = 0 }, // Altered
                    new { Text = forms[822], Value = 1 }, // Origin
                };
            var form_shaymin = new[] {
                    new { Text = forms[492], Value = 0 }, // Land
                    new { Text = forms[823], Value = 1 }, // Sky
                };
            var form_arceus = new[] {
                    new { Text = types[0], Value = 0 }, // Normal
                    new { Text = types[1], Value = 1 }, // Fighting
                    new { Text = types[2], Value = 2 }, // Flying
                    new { Text = types[3], Value = 3 }, // Poison
                    new { Text = types[4], Value = 4 }, // etc
                    new { Text = types[5], Value = 5 },
                    new { Text = types[6], Value = 6 },
                    new { Text = types[7], Value = 7 },
                    new { Text = types[8], Value = 8 },
                    new { Text = types[9], Value = 9 },
                    new { Text = types[10], Value = 10 },
                    new { Text = types[11], Value = 11 },
                    new { Text = types[12], Value = 12 },
                    new { Text = types[13], Value = 13 },
                    new { Text = types[14], Value = 14 },
                    new { Text = types[15], Value = 15 },
                    new { Text = types[16], Value = 16 },
                    new { Text = types[17], Value = 17 },
                };
            var form_basculin = new[] {
                    new { Text = forms[550], Value = 0 }, // Red
                    new { Text = forms[842], Value = 1 }, // Blue
                };
            var form_darmanitan = new[] {
                    new { Text = forms[555], Value = 0 }, // Standard
                    new { Text = forms[843], Value = 1 }, // Zen
                };
            var form_deerling = new[] {
                    new { Text = forms[585], Value = 0 }, // Spring
                    new { Text = forms[844], Value = 1 }, // Summer
                    new { Text = forms[845], Value = 2 }, // Autumn
                    new { Text = forms[846], Value = 3 }, // Winter
                };
            var form_gender = new[] {
                    new { Text = gendersymbols[0], Value = 0 }, // Male
                    new { Text = gendersymbols[1], Value = 1 }, // Female
                };
            var form_therian = new[] {
                    new { Text = forms[641], Value = 0 }, // Incarnate
                    new { Text = forms[852], Value = 1 }, // Therian
                };
            var form_kyurem = new[] {
                    new { Text = types[0], Value = 0 }, // Normal
                    new { Text = forms[853], Value = 1 }, // White
                    new { Text = forms[854], Value = 2 }, // Black
                };
            var form_keldeo = new[] {
                    new { Text = forms[647], Value = 0 }, // Ordinary
                    new { Text = forms[855], Value = 1 }, // Resolute
                };
            var form_meloetta = new[] {
                    new { Text = forms[648], Value = 0 }, // Aria
                    new { Text = forms[856], Value = 1 }, // Pirouette
                };
            var form_genesect = new[] {
                    new { Text = types[0], Value = 0 }, // Normal
                    new { Text = types[10], Value = 1 }, // Douse
                    new { Text = types[12], Value = 2 }, // Shock
                    new { Text = types[9], Value = 3 }, // Burn
                    new { Text = types[14], Value = 4 }, // Chill
                };
            var form_flabebe = new[] {
                    new { Text = forms[669], Value = 0 }, // Red
                    new { Text = forms[884], Value = 1 }, // Yellow
                    new { Text = forms[885], Value = 2 }, // Orange
                    new { Text = forms[886], Value = 3 }, // Blue
                    new { Text = forms[887], Value = 4 }, // White
                };
            var form_floette = new[] {
                    new { Text = forms[669], Value = 0 }, // Red
                    new { Text = forms[884], Value = 1 }, // Yellow
                    new { Text = forms[885], Value = 2 }, // Orange
                    new { Text = forms[886], Value = 3 }, // Blue
                    new { Text = forms[887], Value = 4 }, // White
                    new { Text = forms[888], Value = 5 }, // Eternal
                };
            var form_furfrou = new[] {
                    new { Text = forms[676], Value = 0 }, // Natural
                    new { Text = forms[893], Value = 1 }, // Heart
                    new { Text = forms[894], Value = 2 }, // Star
                    new { Text = forms[895], Value = 3 }, // Diamond
                    new { Text = forms[896], Value = 4 }, // Deputante
                    new { Text = forms[897], Value = 5 }, // Matron
                    new { Text = forms[898], Value = 6 }, // Dandy
                    new { Text = forms[899], Value = 7 }, // La Reine
                    new { Text = forms[900], Value = 8 }, // Kabuki 
                    new { Text = forms[901], Value = 9 }, // Pharaoh
                }; 
            var form_aegislash = new[] {
                    new { Text = forms[681], Value = 0 }, // Shield
                    new { Text = forms[903], Value = 1 }, // Blade
                };
            var form_butterfly = new[] {
                    new { Text = forms[666], Value = 0 }, // Icy Snow
                    new { Text = forms[861], Value = 1 }, // Polar
                    new { Text = forms[862], Value = 2 }, // Tundra
                    new { Text = forms[863], Value = 3 }, // Continental 
                    new { Text = forms[864], Value = 4 }, // Garden
                    new { Text = forms[865], Value = 5 }, // Elegant
                    new { Text = forms[866], Value = 6 }, // Meadow
                    new { Text = forms[867], Value = 7 }, // Modern 
                    new { Text = forms[868], Value = 8 }, // Marine
                    new { Text = forms[869], Value = 9 }, // Archipelago
                    new { Text = forms[870], Value = 10 }, // High-Plains
                    new { Text = forms[871], Value = 11 }, // Sandstorm
                    new { Text = forms[872], Value = 12 }, // River
                    new { Text = forms[873], Value = 13 }, // Monsoon
                    new { Text = forms[874], Value = 14 }, // Savannah 
                    new { Text = forms[875], Value = 15 }, // Sun
                    new { Text = forms[876], Value = 16 }, // Ocean
                    new { Text = forms[877], Value = 17 }, // Jungle
                    new { Text = forms[878], Value = 18 }, // Fancy
                    new { Text = forms[879], Value = 19 }, // Poké Ball
                };
            var form_list = new[] {
                    new { Text = "", Value = 0}, // None
                };
            var form_pump = new[] {
                    new { Text = forms[904], Value = 0 }, // Small
                    new { Text = forms[710], Value = 1 }, // Average
                    new { Text = forms[905], Value = 2 }, // Large
                    new { Text = forms[907], Value = 3 }, // Super
                };
            var form_mega = new[] {
                    new { Text = types[0], Value = 0}, // Normal
                    new { Text = forms[723], Value = 1}, // Mega
                };
            var form_megaxy = new[] {
                    new { Text = types[0], Value = 0}, // Normal
                    new { Text = forms[724], Value = 1}, // Mega X
                    new { Text = forms[725], Value = 2}, // Mega Y
                };

            var form_primal = new[] {
                    new { Text = types[0], Value = 0},
                    new { Text = forms[800], Value = 1},
                };
            var form_hoopa = new[] {
                    new { Text = types[0], Value = 0},
                    new { Text = forms[912], Value = 1},
                };
            var form_pikachu = new[] {
                    new { Text = types[0], Value = 0}, // Normal
                    new { Text = forms[729], Value = 1}, // Rockstar
                    new { Text = forms[730], Value = 2}, // Belle
                    new { Text = forms[731], Value = 3}, // Pop
                    new { Text = forms[732], Value = 4}, // PhD
                    new { Text = forms[733], Value = 5}, // Libre
                    new { Text = forms[734], Value = 6}, // Cosplay
                };

            cb.DataSource = form_list;
            cb.DisplayMember = "Text";
            cb.ValueMember = "Value";

            // Mega List
            int[] mspec = {     // XY
                                   003, 009, 065, 094, 115, 127, 130, 142, 181, 212, 214, 229, 248, 257, 282, 303, 306, 308, 310, 354, 359, 380, 381, 445, 448, 460, 
                                // ORAS
                                015, 018, 080, 208, 254, 260, 302, 319, 323, 334, 362, 373, 376, 384, 428, 475, 531, 719,
                          };
            for (int i = 0; i < mspec.Length; i++)
            {
                if (mspec[i] == species)
                {
                    cb.DataSource = form_mega;
                    cb.Enabled = true; // Mega Form Selection
                    return;
                }
            }

            // MegaXY List
            if ((species == 6) || (species == 150))
            {
                cb.DataSource = form_megaxy;
                cb.Enabled = true; // Mega Form Selection
                return;
            }

            // Regular Form List
            if (species == 025) { form_list = form_pikachu; }
            else if (species == 201) { form_list = form_unown; }
            else if (species == 351) { form_list = form_castform; }
            else if (species == 386) { form_list = form_deoxys; }
            else if (species == 421) { form_list = form_cherrim; }
            else if (species == 479) { form_list = form_rotom; }
            else if (species == 487) { form_list = form_giratina; }
            else if (species == 492) { form_list = form_shaymin; }
            else if (species == 493) { form_list = form_arceus; }
            else if (species == 550) { form_list = form_basculin; }
            else if (species == 555) { form_list = form_darmanitan; }
            else if (species == 646) { form_list = form_kyurem; }
            else if (species == 647) { form_list = form_keldeo; }
            else if (species == 648) { form_list = form_meloetta; }
            else if (species == 649) { form_list = form_genesect; }
            else if (species == 676) { form_list = form_furfrou; }
            else if (species == 681) { form_list = form_aegislash; }
            else if (species == 670) { form_list = form_floette; }

            else if ((species == 669) || (species == 671)) { form_list = form_flabebe; }
            else if ((species == 412) || (species == 413)) { form_list = form_burmy; }
            else if ((species == 422) || (species == 423)) { form_list = form_shellos; }
            else if ((species == 585) || (species == 586)) { form_list = form_deerling; }
            else if ((species == 710) || (species == 711)) { form_list = form_pump; }

            else if ((species == 666) || (species == 665) || (species == 664)) { form_list = form_butterfly; }
            else if ((species == 592) || (species == 593) || (species == 678)) { form_list = form_gender; }
            else if ((species == 641) || (species == 642) || (species == 645)) { form_list = form_therian; }

            // ORAS
            else if (species == 382 || species == 383) { form_list = form_primal; }
            else if (species == 720) { form_list = form_hoopa; }

            else
            {
                cb.Enabled = false;
                return;
            };

            cb.DataSource = form_list;
            cb.Enabled = true;
        }
        private void setGenderLabel()
        {
            if (genderflag == 0)
                Label_Gender.Text = gendersymbols[0];    // Male
            else if (genderflag == 1)
                Label_Gender.Text = gendersymbols[1];    // Female
            else
                Label_Gender.Text = gendersymbols[2];    // Genderless
        }
        private void setMarkings()
        {
            PictureBox[] pba = { PB_Mark1, PB_Mark2, PB_Mark3, PB_Mark4, PB_Mark5, PB_Mark6 };
            CheckBox[] cba = { CHK_Circle, CHK_Triangle, CHK_Square, CHK_Heart, CHK_Star, CHK_Diamond };
            for (int i = 0; i < 6; i++)
                pba[i].Image = Util.ChangeOpacity(pba[i].InitialImage, (float)(Convert.ToUInt16(cba[i].Checked)) * 0.9 + 0.1);

            PB_MarkShiny.Image = Util.ChangeOpacity(PB_MarkShiny.InitialImage, (float)(Convert.ToUInt16(!BTN_Shinytize.Enabled)) * 0.9 + 0.1);
            PB_MarkCured.Image = Util.ChangeOpacity(PB_MarkCured.InitialImage, (float)(Convert.ToUInt16(CHK_Cured.Checked)) * 0.9 + 0.1);
            int gameindex = Util.getIndex(CB_GameOrigin);
            PB_MarkPentagon.Image = Util.ChangeOpacity(PB_MarkPentagon.InitialImage, (float)(Convert.ToUInt16(gameindex == 24 || gameindex == 25 || gameindex == 26 || gameindex == 27)) * 0.9 + 0.1);
        }
        // Label Shortcut Tweaks
        private void Label_Friendship_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control) // prompt to reset
            {
                if (buff[0x93] == 0)
                    TB_Friendship.Text = buff[0xCA].ToString();
                else TB_Friendship.Text = buff[0xA2].ToString();

                return;
            }
            else if (TB_Friendship.Text == "255") // if it's maxed, set it to base
                TB_Friendship.Text = PKX.getBaseFriendship(Util.getIndex(CB_Species)).ToString();
            else // not reset, not maxed, so max
                TB_Friendship.Text = "255";
        }
        private void Label_Gender_Click(object sender, EventArgs e)
        {
            // Get Gender Threshold
            species = Util.getIndex(CB_Species);
            PersonalParser PP = PKX.PersonalGetter;
            PersonalParser.Personal MonData = PP.GetPersonal(species);
            gt = MonData.GenderRatio;

            if (gt == 255 || gt == 0 || gt == 254) // Single gender/genderless
                return;

            if (gt < 255) // If not a single gender(less) species:
            {
                if (PKX.getGender(Label_Gender.Text) == 0) // ♂
                    Label_Gender.Text = gendersymbols[1]; // ♀
                else
                    Label_Gender.Text = gendersymbols[0]; // ♂

                if (PKX.getGender(CB_Form.Text) == 0 && Label_Gender.Text != gendersymbols[0])
                    CB_Form.SelectedIndex = 1;
                else if (PKX.getGender(CB_Form.Text) == 1 && Label_Gender.Text != gendersymbols[1])
                    CB_Form.SelectedIndex = 0;
            }

            if (species == 592 || species == 593)
                CB_Form.SelectedIndex = PKX.getGender(Label_Gender.Text);
        }
        private void Label_PPups_Click(object sender, EventArgs e)
        {
            CB_PPu1.SelectedIndex = 3 * (Convert.ToInt16(!((ModifierKeys == Keys.Control) || Util.getIndex(CB_Move1) == 0)));
            CB_PPu2.SelectedIndex = 3 * (Convert.ToInt16(!((ModifierKeys == Keys.Control) || Util.getIndex(CB_Move2) == 0)));
            CB_PPu3.SelectedIndex = 3 * (Convert.ToInt16(!((ModifierKeys == Keys.Control) || Util.getIndex(CB_Move3) == 0)));
            CB_PPu4.SelectedIndex = 3 * (Convert.ToInt16(!((ModifierKeys == Keys.Control) || Util.getIndex(CB_Move4) == 0)));
        }
        private void Label_Marking_Click(object sender, EventArgs e)
        {
            PictureBox[] pba = { PB_Mark1, PB_Mark2, PB_Mark3, PB_Mark4, PB_Mark5, PB_Mark6 };
            CheckBox[] cba = { CHK_Circle, CHK_Triangle, CHK_Square, CHK_Heart, CHK_Star, CHK_Diamond };

            CheckBox cb = cba[Array.IndexOf(pba, sender as PictureBox)];
            cb.Checked = !cb.Checked;
            setMarkings();
        }
        private void Label_OT_Click(object sender, EventArgs e)
        {
            string OT = Util.TrimFromZero(Encoding.Unicode.GetString(savefile, SaveGame.TrainerCard + 0x48 + savindex * 0x7F000, 0x1A));
            if (OT.Length > 0)
            {
                TB_OT.Text = OT;
                int savshift = 0x7F000 * savindex;
                // Set Gender Label
                int g6trgend = savefile[SaveGame.TrainerCard + 0x5 + savshift];
                if (g6trgend == 1)
                    Label_OTGender.Text = gendersymbols[1]; // ♀
                else Label_OTGender.Text = gendersymbols[0]; // ♂

                // Get TID/SID
                TB_TID.Text = BitConverter.ToUInt16(savefile, SaveGame.TrainerCard + 0 + savshift).ToString();
                TB_SID.Text = BitConverter.ToUInt16(savefile, SaveGame.TrainerCard + 2 + savshift).ToString();
                int game = savefile[SaveGame.TrainerCard + 0x4 + savshift];
                int subreg = savefile[SaveGame.TrainerCard + 0x26 + savshift];
                int country = savefile[SaveGame.TrainerCard + 0x27 + savshift];
                int _3DSreg = savefile[SaveGame.TrainerCard + 0x2C + savshift];
                int lang = savefile[SaveGame.TrainerCard + 0x2D + savshift];

                // CB_GameOrigin.SelectedValue = game;

                CB_SubRegion.SelectedValue = subreg;
                CB_Country.SelectedValue = country;
                CB_3DSReg.SelectedValue = _3DSreg;
                CB_Language.SelectedValue = lang;
                updateNickname(null, null);
            }
        }
        private void Label_CT_Click(object sender, EventArgs e)
        {
            string OT = Util.TrimFromZero(Encoding.Unicode.GetString(savefile, 0x19448 + savindex * 0x7F000, 0x1A));
            if (OT.Length > 0)
            {
                TB_OTt2.Text = OT;
                // Set Gender Label
                int g6trgend = savefile[0x19405 + savindex * 0x7F000];
                if (g6trgend == 1)
                    Label_CTGender.Text = gendersymbols[1]; // ♀
                else Label_CTGender.Text = gendersymbols[0]; // ♂
            }
        }
        private void Label_TrainerGender_Click(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            if (lbl.Text == "") 
                return;
            else // set gender label (toggle M/F)
                lbl.Text = (PKX.getGender(lbl.Text) == 0) ? gendersymbols[1] : gendersymbols[0];
        }
        // Prompted Updates of PKX Functions // 
        public void setCountry(object sender)
        {
            #region country table
            var country_list = new[] {
                            new { Text = "---", Value = 0 },
                            new { Text = "Albania", Value = 64 },
                            new { Text = "Andorra", Value = 122 },
                            new { Text = "Anguilla", Value = 8 },
                            new { Text = "Antigua and Barbuda", Value = 9 },
                            new { Text = "Argentina", Value = 10 },
                            new { Text = "Aruba", Value = 11 },
                            new { Text = "Australia", Value = 65 },
                            new { Text = "Austria", Value = 66 },
                            new { Text = "Azerbaijan", Value = 113 },
                            new { Text = "Bahamas", Value = 12 },
                            new { Text = "Barbados", Value = 13 },
                            new { Text = "Belgium", Value = 67 },
                            new { Text = "Belize", Value = 14 },
                            new { Text = "Bermuda", Value = 186 },
                            new { Text = "Bolivia", Value = 15 },
                            new { Text = "Bosnia and Herzegovina", Value = 68 },
                            new { Text = "Botswana", Value = 69 },
                            new { Text = "Brazil", Value = 16 },
                            new { Text = "British Virgin Islands", Value = 17 },
                            new { Text = "Bulgaria", Value = 70 },
                            new { Text = "Canada", Value = 18 },
                            new { Text = "Cayman Islands", Value = 19 },
                            new { Text = "Chad", Value = 117 },
                            new { Text = "Chile", Value = 20 },
                            new { Text = "China", Value = 160 },
                            new { Text = "Colombia", Value = 21 },
                            new { Text = "Costa Rica", Value = 22 },
                            new { Text = "Croatia", Value = 71 },
                            new { Text = "Cyprus", Value = 72 },
                            new { Text = "Czech Republic", Value = 73 },
                            new { Text = "Denmark (Kingdom of)", Value = 74 },
                            new { Text = "Djibouti", Value = 120 },
                            new { Text = "Dominica", Value = 23 },
                            new { Text = "Dominican Republic", Value = 24 },
                            new { Text = "Ecuador", Value = 25 },
                            new { Text = "El Salvador", Value = 26 },
                            new { Text = "Eritrea", Value = 119 },
                            new { Text = "Estonia", Value = 75 },
                            new { Text = "Finland", Value = 76 },
                            new { Text = "France", Value = 77 },
                            new { Text = "French Guiana", Value = 27 },
                            new { Text = "Germany", Value = 78 },
                            new { Text = "Gibraltar", Value = 123 },
                            new { Text = "Greece", Value = 79 },
                            new { Text = "Grenada", Value = 28 },
                            new { Text = "Guadeloupe", Value = 29 },
                            new { Text = "Guatemala", Value = 30 },
                            new { Text = "Guernsey", Value = 124 },
                            new { Text = "Guyana", Value = 31 },
                            new { Text = "Haiti", Value = 32 },
                            new { Text = "Honduras", Value = 33 },
                            new { Text = "Hong Kong", Value = 144 },
                            new { Text = "Hungary", Value = 80 },
                            new { Text = "Iceland", Value = 81 },
                            new { Text = "India", Value = 169 },
                            new { Text = "Ireland", Value = 82 },
                            new { Text = "Isle of Man", Value = 125 },
                            new { Text = "Italy", Value = 83 },
                            new { Text = "Jamaica", Value = 34 },
                            new { Text = "Japan", Value = 1 },
                            new { Text = "Jersey", Value = 126 },
                            new { Text = "Latvia", Value = 84 },
                            new { Text = "Lesotho", Value = 85 },
                            new { Text = "Liechtenstein", Value = 86 },
                            new { Text = "Lithuania", Value = 87 },
                            new { Text = "Luxembourg", Value = 88 },
                            new { Text = "Macedonia (Republic of)", Value = 89 },
                            new { Text = "Malaysia", Value = 156 },
                            new { Text = "Mali", Value = 115 },
                            new { Text = "Malta", Value = 90 },
                            new { Text = "Martinique", Value = 35 },
                            new { Text = "Mauritania", Value = 114 },
                            new { Text = "Mexico", Value = 36 },
                            new { Text = "Monaco", Value = 127 },
                            new { Text = "Montenegro", Value = 91 },
                            new { Text = "Montserrat", Value = 37 },
                            new { Text = "Mozambique", Value = 92 },
                            new { Text = "Namibia", Value = 93 },
                            new { Text = "Netherlands", Value = 94 },
                            new { Text = "Netherlands Antilles", Value = 38 },
                            new { Text = "New Zealand", Value = 95 },
                            new { Text = "Nicaragua", Value = 39 },
                            new { Text = "Niger", Value = 116 },
                            new { Text = "Norway", Value = 96 },
                            new { Text = "Panama", Value = 40 },
                            new { Text = "Paraguay", Value = 41 },
                            new { Text = "Peru", Value = 42 },
                            new { Text = "Poland", Value = 97 },
                            new { Text = "Portugal", Value = 98 },
                            new { Text = "Romania", Value = 99 },
                            new { Text = "Russia", Value = 100 },
                            new { Text = "San Marino", Value = 184 },
                            new { Text = "Saudi Arabia", Value = 174 },
                            new { Text = "Serbia and Kosovo", Value = 101 },
                            new { Text = "Singapore", Value = 153 },
                            new { Text = "Slovakia", Value = 102 },
                            new { Text = "Slovenia", Value = 103 },
                            new { Text = "Somalia", Value = 121 },
                            new { Text = "South Africa", Value = 104 },
                            new { Text = "South Korea", Value = 136 },
                            new { Text = "Spain", Value = 105 },
                            new { Text = "St. Kitts and Nevis", Value = 43 },
                            new { Text = "St. Lucia", Value = 44 },
                            new { Text = "St. Vincent and the Grenadines", Value = 45 },
                            new { Text = "Sudan", Value = 118 },
                            new { Text = "Suriname", Value = 46 },
                            new { Text = "Swaziland", Value = 106 },
                            new { Text = "Sweden", Value = 107 },
                            new { Text = "Switzerland", Value = 108 },
                            new { Text = "Taiwan", Value = 128 },
                            new { Text = "Trinidad and Tobago", Value = 47 },
                            new { Text = "Turkey", Value = 109 },
                            new { Text = "Turks and Caicos Islands", Value = 48 },
                            new { Text = "U.A.E.", Value = 168 },
                            new { Text = "United Kingdom", Value = 110 },
                            new { Text = "United States", Value = 49 },
                            new { Text = "Uruguay", Value = 50 },
                            new { Text = "US Virgin Islands", Value = 51 },
                            new { Text = "Vatican City", Value = 185 },
                            new { Text = "Venezuela", Value = 52 },
                            new { Text = "Zambia", Value = 111 },
                            new { Text = "Zimbabwe", Value = 112 },
            };
            #endregion
            ComboBox CB = sender as ComboBox;
            CB.DataSource = country_list;
            CB.DisplayMember = "Text";
            CB.ValueMember = "Value";
        }
        private void updateEXPLevel(object sender, EventArgs e)
        {
            if ((TB_EXP.Focused == true) && (TB_EXP.Enabled == true))
            {
                // Change the Level
                TB_Level.Enabled = false;
                int level;
                uint exp = Util.ToUInt32(TB_EXP);
                if (Util.ToInt32(TB_EXP.Text) == 0) { level = 1; }
                else level = PKX.getLevel(Util.getIndex(CB_Species), ref exp);
                TB_Level.Text = level.ToString();
                if (!MT_Level.Visible || level < 100)
                    TB_EXP.Text = exp.ToString();
                if (MT_Level.Visible && level < 101 && Util.ToInt32(MT_Level.Text) < 101)
                    MT_Level.Text = level.ToString();

                TB_Level.Enabled = true;
            }
            else if ((TB_Level.Focused == true) && (TB_Level.Enabled == true))// TB_Level is focused
            {
                // Change the XP
                TB_EXP.Enabled = false;
                int level = Util.ToInt32(TB_Level.Text);
                if (level > 100) 
                { TB_Level.Text = "100"; level = 100; }

                {
                    // Valid Level, recalculate EXP
                    TB_EXP.Text = PKX.getEXP(level, Util.getIndex(CB_Species)).ToString();
                    TB_Level.BackColor = Color.White;
                }
                TB_EXP.Enabled = true;
            }
            else if (MT_Level.Focused == true)
            {
                int level = Util.ToInt32(MT_Level.Text); if (level > 255) level = 255;
                TB_EXP.Text = PKX.getEXP(level, Util.getIndex(CB_Species)).ToString();
            }
            updateStats();
        }
        private void updateIVs(object sender, EventArgs e)
        {
            if (sender != null)
                if (Util.ToInt32((sender as MaskedTextBox).Text) > 31)
                    (sender as MaskedTextBox).Text = "31";

            int ivtotal, HP_IV, ATK_IV, DEF_IV, SPA_IV, SPD_IV, SPE_IV;
            HP_IV = Util.ToInt32(TB_HPIV.Text);
            ATK_IV = Util.ToInt32(TB_ATKIV.Text);
            DEF_IV = Util.ToInt32(TB_DEFIV.Text);
            SPA_IV = Util.ToInt32(TB_SPAIV.Text);
            SPD_IV = Util.ToInt32(TB_SPDIV.Text);
            SPE_IV = Util.ToInt32(TB_SPEIV.Text);

            int[] iva = new int[] { HP_IV, ATK_IV, DEF_IV, SPE_IV, SPA_IV, SPD_IV };
            MaskedTextBox[] ivt = { TB_HPIV, TB_ATKIV, TB_DEFIV, TB_SPEIV, TB_SPAIV, TB_SPDIV };

            int HPTYPE = (15 * ((HP_IV & 1) + 2 * (ATK_IV & 1) + 4 * (DEF_IV & 1) + 8 * (SPE_IV & 1) + 16 * (SPA_IV & 1) + 32 * (SPD_IV & 1))) / 63;
            Label_HPTYPE.Text = types[HPTYPE+1]; // type array has normal at index 0, so we offset by 1

            ivtotal = HP_IV + ATK_IV + DEF_IV + SPA_IV + SPD_IV + SPE_IV;
            TB_IVTotal.Text = ivtotal.ToString();

            // Potential Reading
            if (!unicode)
            {
                if (ivtotal <= 90)
                    L_Potential.Text = "★☆☆☆";
                else if (ivtotal <= 120)
                    L_Potential.Text = "★★☆☆";
                else if (ivtotal <= 150)
                    L_Potential.Text = "★★★☆";
                else
                    L_Potential.Text = "★★★★";
            }
            else
            {
                if (ivtotal <= 90)
                    L_Potential.Text = "+";
                else if (ivtotal <= 120)
                    L_Potential.Text = "++";
                else if (ivtotal <= 150)
                    L_Potential.Text = "+++";
                else
                    L_Potential.Text = "++++";
            }

            // Characteristic with PID%6
            int pm6 = (int)(Util.getHEXval(TB_PID) % 6); // PID MOD 6
            int maxIV = iva.Max();
            int pm6stat = 0;

            for (int i = 0; i < 6; i++)
            {
                pm6stat = (pm6 + i) % 6;
                if (iva[pm6stat] == maxIV)
                    break;  // P%6 is this stat
            }

            L_Characteristic.Text = characteristics[pm6stat * 5 + maxIV % 5];
            updateStats();
        }
        private void updateEVs(object sender, EventArgs e)
        {
            if (sender != null)
                if (Util.ToInt32((sender as MaskedTextBox).Text) > 252)
                    (sender as MaskedTextBox).Text = "252";

            int evtotal, HP_EV, ATK_EV, DEF_EV, SPA_EV, SPD_EV, SPE_EV;
            HP_EV = Util.ToInt32(TB_HPEV.Text);
            ATK_EV = Util.ToInt32(TB_ATKEV.Text);
            DEF_EV = Util.ToInt32(TB_DEFEV.Text);
            SPA_EV = Util.ToInt32(TB_SPAEV.Text);
            SPD_EV = Util.ToInt32(TB_SPDEV.Text);
            SPE_EV = Util.ToInt32(TB_SPEEV.Text);

            int[] eva = new int[] { HP_EV, ATK_EV, DEF_EV, SPA_EV, SPD_EV, SPE_EV };

            evtotal = HP_EV + ATK_EV + DEF_EV + SPA_EV + SPD_EV + SPE_EV;

            if (evtotal > 510) // Background turns Red
                 TB_EVTotal.BackColor = Color.Red;
            else if (evtotal == 510) // Maximum EVs
                 TB_EVTotal.BackColor = Color.Honeydew;
            else TB_EVTotal.BackColor = Color.WhiteSmoke;

            TB_EVTotal.Text = evtotal.ToString();
            updateStats();
        }
        private void updateRandomIVs(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                // Max IVs
                TB_HPIV.Text = 31.ToString();
                TB_ATKIV.Text = 31.ToString();
                TB_DEFIV.Text = 31.ToString();
                TB_SPAIV.Text = 31.ToString();
                TB_SPDIV.Text = 31.ToString();
                TB_SPEIV.Text = 31.ToString();
            }
            else
            {
                TB_HPIV.Text = (Util.rnd32() & 0x1F).ToString();
                TB_ATKIV.Text = (Util.rnd32() & 0x1F).ToString();
                TB_DEFIV.Text = (Util.rnd32() & 0x1F).ToString();
                TB_SPAIV.Text = (Util.rnd32() & 0x1F).ToString();
                TB_SPDIV.Text = (Util.rnd32() & 0x1F).ToString();
                TB_SPEIV.Text = (Util.rnd32() & 0x1F).ToString();
            }
        }
        private void updateRandomEVs(object sender, EventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                // Max IVs
                TB_HPEV.Text = 0.ToString();
                TB_ATKEV.Text = 0.ToString();
                TB_DEFEV.Text = 0.ToString();
                TB_SPAEV.Text = 0.ToString();
                TB_SPDEV.Text = 0.ToString();
                TB_SPEEV.Text = 0.ToString();
                return;
            }
            else
            {
                byte[] evs = PKX.getRandomEVs();
                TB_HPEV.Text = evs[0].ToString();
                TB_ATKEV.Text = evs[1].ToString();
                TB_DEFEV.Text = evs[2].ToString();
                TB_SPAEV.Text = evs[3].ToString();
                TB_SPDEV.Text = evs[4].ToString();
                TB_SPEEV.Text = evs[5].ToString();
            }
        }
        private void updateRandomPID(object sender, EventArgs e)
        {
            TB_PID.Text = PKX.getRandomPID(Util.getIndex(CB_Species),PKX.getGender(Label_Gender.Text)).ToString("X8");
        }
        private void updateRandomEC(object sender, EventArgs e)
        {
            TB_EC.Text = Util.rnd32().ToString("X8");
        }
        private void updateHackedStats(object sender, EventArgs e)
        {
            Stat_HP.Enabled =
                Stat_ATK.Enabled =
                Stat_DEF.Enabled =
                Stat_SPA.Enabled =
                Stat_SPD.Enabled =
                Stat_SPE.Enabled = CHK_HackedStats.Checked;
        }
        private void update255_MTB(object sender, EventArgs e)
        {
            MaskedTextBox mtb = sender as MaskedTextBox;
            try
            {
                if (Util.ToInt32((sender as MaskedTextBox).Text) > 255)
                    (sender as MaskedTextBox).Text = "255";
            }
            catch { mtb.Text = "0"; }
        }
        private void update255_TB(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            try
            {
                if (Util.ToInt32((sender as TextBox).Text) > 255)
                    (sender as TextBox).Text = "255";
            }
            catch { tb.Text = "0"; }
        }
        private void updateForm(object sender, EventArgs e)
        {
            updateStats();
            // Repopulate Abilities if Species Form has different abilities
            updateAbilityList(TB_AbilityNumber, Util.getIndex(CB_Species), CB_Ability, CB_Form);

            // If form has a single gender, account for it.
            if (PKX.getGender(CB_Form.Text) == 0)
                Label_Gender.Text = gendersymbols[0];
            else if (PKX.getGender(CB_Form.Text) == 1)
                Label_Gender.Text = gendersymbols[1];
        }
        private void updatePP(object sender, EventArgs e)
        {
            TB_PP1.Text = (PKX.getMovePP(Util.getIndex(CB_Move1), CB_PPu1.SelectedIndex)).ToString();
            TB_PP2.Text = (PKX.getMovePP(Util.getIndex(CB_Move2), CB_PPu2.SelectedIndex)).ToString();
            TB_PP3.Text = (PKX.getMovePP(Util.getIndex(CB_Move3), CB_PPu3.SelectedIndex)).ToString();
            TB_PP4.Text = (PKX.getMovePP(Util.getIndex(CB_Move4), CB_PPu4.SelectedIndex)).ToString();
        }
        private void updatePKRSstrain(object sender, EventArgs e)
        {
            if (CB_PKRSStrain.SelectedIndex == 0)
            {
                // Never Infected
                CB_PKRSDays.SelectedValue = 0;
                CHK_Cured.Checked = false;
                CHK_Infected.Checked = false;
            }
        }
        private void updatePKRSdays(object sender, EventArgs e)
        {
            if (CB_PKRSDays.SelectedIndex == 0)
            {
                // If no days are selected
                if (CB_PKRSStrain.SelectedIndex == 0)
                {
                    // Never Infected
                    CHK_Cured.Checked = false;
                    CHK_Infected.Checked = false;
                }
                else CHK_Cured.Checked = true;
            }
        }
        private void updateSpecies(object sender, EventArgs e)
        {
            // Change Species Prompted
            int species = Util.getIndex(CB_Species);
            int level = Util.ToInt32(TB_Level.Text);
            if (MT_Level.Visible) level = Util.ToInt32(MT_Level.Text);

            // Get Forms for Given Species
            setForms(species, CB_Form);

            // Recalculate EXP for Given Level
            uint exp = PKX.getEXP(level, species);
            TB_EXP.Text = exp.ToString();

            // Check for Gender Changes
            // Get Gender Threshold
            species = Util.getIndex(CB_Species);
            PersonalParser.Personal MonData = PKX.PersonalGetter.GetPersonal(species);
            gt = MonData.GenderRatio;

            if (gt == 255)      // Genderless
                genderflag = 2;
            else if (gt == 254) // Female Only
                genderflag = 1;
            else if (gt == 0) // Male Only
                genderflag = 0;
            else // get gender from old PID correlation
                genderflag = ((Util.getHEXval(TB_PID) & 0xFF) <= gt) ? 1 : 0;

            setGenderLabel();
            updateAbilityList(TB_AbilityNumber, Util.getIndex(CB_Species), CB_Ability, CB_Form);
            updateForm(null, null);

            // If species changes and no nickname, set the new name == speciesName.
            if (!CHK_Nicknamed.Checked)
                updateNickname(sender, e);
        }
        private void updateOriginGame(object sender, EventArgs e)
        {
            int gameorigin = 0;

            // Error handling for unset field
			try 
			{
				gameorigin = Util.ToInt32(CB_GameOrigin.SelectedValue.ToString()); 
			}
            catch { gameorigin = 0; }

            if ((gameorigin <= 12) && (gameorigin >= 7))
            {
                // Game Originates In Gen 4; Enable Encounter Type
                CB_EncounterType.Enabled = true;
                Label_EncounterType.Enabled = true;
            }
            else
            {
                CB_EncounterType.Enabled = false;
                Label_EncounterType.Enabled = false;
                CB_EncounterType.SelectedIndex = 0;
            }
            updateLocations(gameorigin);
            setMarkings();
            setIsShiny();
        }
        private void updateLocations(int gameorigin)
        {
            if (gameorigin < 24 && origintrack != "Past") // Load Past Gen Locations
            {
                #region BW2 Met Locations
                {
                    // Allowed Met Locations
                    int[] metlocs = { 0, 60002, 30003 };

                    // Set up
                    List<cbItem> met_list = new List<cbItem>();

                    for (int i = 0; i < metlocs.Length; i++) // add entries to the top
                    {
                        cbItem ncbi = new cbItem();
                        int locval = metlocs[i];
                        string loctext = "";

                        if (locval < 30000)
                            loctext = metBW2_00000[locval];
                        else if (locval < 40000)
                            loctext = metBW2_30000[locval % 10000 - 1];
                        else if (locval < 60000)
                            loctext = metBW2_40000[locval % 10000 - 1];
                        else
                            loctext = metBW2_60000[locval % 10000 - 1];

                        ncbi.Text = loctext;
                        ncbi.Value = locval;
                        met_list.Add(ncbi);
                    }

                    metlocs = new int[] 
                    { 
                        1,2,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135,136,137,139,140,141,142,143,144,145,146,147,148,149,150,151,152,153,
                        30001,30002,30004,30005,30006,30007,30008,30010,30011,30012,30013,30014,30015,
                        40001,40002,40003,40004,40005,40006,40007,40008,40009,40010,40011,40012,40013,40014,40015,40016,40017,40018,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40032,40033,40034,40035,40036,40037,40038,40039,40040,40041,40042,40043,40044,40045,40046,40047,40048,40049,40050,40051,40052,40053,40054,40055,40056,40057,40058,40059,40060,40061,40062,40063,40064,40065,40066,40067,40068,40069,40070,40071,40072,40073,40074,40075,40076,40077,40078,40079,40080,40081,40082,40083,40084,40085,40086,40087,40088,40089,40090,40091,40092,40093,40094,40095,40096,40097,40098,40099,40100,40101,40102,40103,40104,40105,40106,40107,40108,40109,
                        60001,60003
                    };

                    // Sort the Rest based on String Name
                    string[] lt00000 = new string[metBW2_00000.Length];
                    string[] lt30000 = new string[metBW2_30000.Length];
                    string[] lt40000 = new string[metBW2_40000.Length];
                    string[] lt60000 = new string[metBW2_60000.Length];
                    Array.Copy(metBW2_00000, lt00000, metBW2_00000.Length);
                    Array.Copy(metBW2_30000, lt30000, metBW2_30000.Length);
                    Array.Copy(metBW2_40000, lt40000, metBW2_40000.Length);
                    Array.Copy(metBW2_60000, lt60000, metBW2_60000.Length);
                    Array.Sort(lt00000);
                    Array.Sort(lt30000);
                    Array.Sort(lt40000);
                    Array.Sort(lt60000);

                    // Add the rest of the 00000
                    for (int i = 0; i < lt00000.Length; i++)
                    {
                        int locnum = Array.IndexOf(metlocs, Array.IndexOf(metBW2_00000, lt00000[i]) + 00000);
                        if (locnum > 0)	// If the location is allowed (if found, >0)
                        {
                            cbItem ncbi = new cbItem();
                            ncbi.Text = lt00000[i];
                            ncbi.Value = metlocs[locnum];
                            met_list.Add(ncbi);
                        }
                    }
                    // Add the rest of the 30000
                    for (int i = 0; i < lt30000.Length; i++)
                    {
                        int locnum = Array.IndexOf(metlocs, Array.IndexOf(metBW2_30000, lt30000[i]) + 30001);
                        if (locnum > 0)	// If the location is allowed (if found, >0)
                        {
                            cbItem ncbi = new cbItem();
                            ncbi.Text = lt30000[i];
                            ncbi.Value = metlocs[locnum];
                            met_list.Add(ncbi);
                        }
                    }
                    // Add the rest of the 40000
                    for (int i = 0; i < lt40000.Length; i++)
                    {
                        int locnum = Array.IndexOf(metlocs, Array.IndexOf(metBW2_40000, lt40000[i]) + 40001);
                        if (locnum > 0)	// If the location is allowed (if found, >0)
                        {
                            cbItem ncbi = new cbItem();
                            ncbi.Text = lt40000[i];
                            ncbi.Value = metlocs[locnum];
                            met_list.Add(ncbi);
                        }
                    }
                    // Add the rest of the 60000
                    for (int i = 0; i < lt60000.Length; i++)
                    {
                        int locnum = Array.IndexOf(metlocs, Array.IndexOf(metBW2_60000, lt60000[i]) + 60001);
                        if (locnum > 0)	// If the location is allowed (if found, >0)
                        {
                            cbItem ncbi = new cbItem();
                            ncbi.Text = lt60000[i];
                            ncbi.Value = metlocs[locnum];
                            met_list.Add(ncbi);
                        }
                    }

                    CB_MetLocation.DataSource = met_list;
                    CB_MetLocation.DisplayMember = "Text";
                    CB_MetLocation.ValueMember = "Value";
                    CB_EggLocation.DataSource = new BindingSource(met_list, null);
                    origintrack = "Past";
                    CB_EggLocation.DisplayMember = "Text";
                    CB_EggLocation.ValueMember = "Value";
                    CB_EggLocation.SelectedValue = 0;
                    if (gameorigin < 20)
                        CB_MetLocation.SelectedValue = 30001; // Transporter
                    else CB_MetLocation.SelectedValue = 60001; // Stranger
                }
                #endregion
            }
            else if (gameorigin > 23 && (origintrack != "XY"))
            {
                #region XY Met Locations
                {
                    // Allowed Met Locations
                    int[] metlocs = { 0, 60002, 30002 };

                    // Set up
                    List<cbItem> met_list = new List<cbItem>();

                    for (int i = 0; i < metlocs.Length; i++) // add entries to the top
                    {
                        cbItem ncbi = new cbItem();
                        int locval = metlocs[i];
                        string loctext = "";

                        if (locval < 30000)
                            loctext = metXY_00000[locval];
                        else if (locval < 40000)
                            loctext = metXY_30000[locval % 10000 - 1];
                        else if (locval < 60000)
                            loctext = metXY_40000[locval % 10000 - 1];
                        else
                            loctext = metXY_60000[locval % 10000 - 1];

                        ncbi.Text = loctext;
                        ncbi.Value = locval;
                        met_list.Add(ncbi);
                    }

                    metlocs = new int[] 
                    { 
                        2,6,8,10,12,14,16,17,18,20,22,24,26,28,30,32,34,36,38,40,42,44,46,48,50,52,54,56,58,60,62,64,66,68,70,72,74,76,78,82,84,86,88,90,92,94,96,98,100,102,104,106,108,110,112,114,116,118,120,122,124,126,128,130,132,134,136,138,140,142,144,146,148,150,152,154,156,158,160,162,164,166,168,
                        // ORAS
                        170,172,174,176,178,180,182,184,186,188,190,192,194,196,198,200,202,204,206,208,210,212,214,216,218,220,222,224,226,228,230,232,234,236,238,240,242,244,246,248,250,252,254,256,258,260,262,264,266,268,270,272,274,276,278,280,282,284,286,288,290,292,294,296,298,300,302,304,306,308,310,312,314,316,318,320,322,324,326,328,330,332,334,336,338,340,342,344,346,348,350,352,354,

                        30001,30003,30004,30005,30006,30007,30008,30009,30010,30011,
                        40001,40002,40003,40004,40005,40006,40007,40008,40009,40010,40011,40012,40013,40014,40015,40016,40017,40018,40019,40020,40021,40022,40023,40024,40025,40026,40027,40028,40029,40030,40031,40032,40033,40034,40035,40036,40037,40038,40039,40040,40041,40042,40043,40044,40045,40046,40047,40048,40049,40050,40051,40052,40053,40054,40055,40056,40057,40058,40059,40060,40061,40062,40063,40064,40065,40066,40067,40068,40069,40070,40071,40072,40073,40074,40075,40076,40077,40078,40079,
                        60001,60003,
                        // ORAS
                        60004,
                    };

                    // Sort the Rest based on String Name
                    string[] lt00000 = new string[metXY_00000.Length];
                    string[] lt30000 = new string[metXY_30000.Length];
                    string[] lt40000 = new string[metXY_40000.Length];
                    string[] lt60000 = new string[metXY_60000.Length];
                    Array.Copy(metXY_00000, lt00000, metXY_00000.Length);
                    Array.Copy(metXY_30000, lt30000, metXY_30000.Length);
                    Array.Copy(metXY_40000, lt40000, metXY_40000.Length);
                    Array.Copy(metXY_60000, lt60000, metXY_60000.Length);
                    Array.Sort(lt00000);
                    Array.Sort(lt30000);
                    Array.Sort(lt40000);
                    Array.Sort(lt60000);

                    // Add the rest of the 00000
                    for (int i = 0; i < lt00000.Length; i++)
                    {
                        int locnum = Array.IndexOf(metlocs, Array.IndexOf(metXY_00000, lt00000[i]) + 00000);
                        if (locnum > 0)	// If the location is allowed (if found, >0)
                        {
                            cbItem ncbi = new cbItem();
                            ncbi.Text = lt00000[i];
                            ncbi.Value = metlocs[locnum];
                            met_list.Add(ncbi);
                        }
                    }
                    // Add the rest of the 30000
                    for (int i = 0; i < lt30000.Length; i++)
                    {
                        int locnum = Array.IndexOf(metlocs, Array.IndexOf(metXY_30000, lt30000[i]) + 30001);
                        if (locnum > 0)	// If the location is allowed (if found, >0)
                        {
                            cbItem ncbi = new cbItem();
                            ncbi.Text = lt30000[i];
                            ncbi.Value = metlocs[locnum];
                            met_list.Add(ncbi);
                        }
                    }
                    // Add the rest of the 40000
                    for (int i = 0; i < lt40000.Length; i++)
                    {
                        int locnum = Array.IndexOf(metlocs, Array.IndexOf(metXY_40000, lt40000[i]) + 40001);
                        if (locnum > 0)	// If the location is allowed (if found, >0)
                        {
                            cbItem ncbi = new cbItem();
                            ncbi.Text = lt40000[i];
                            ncbi.Value = metlocs[locnum];
                            met_list.Add(ncbi);
                        }
                    }
                    // Add the rest of the 60000
                    for (int i = 0; i < lt60000.Length; i++)
                    {
                        int locnum = Array.IndexOf(metlocs, Array.IndexOf(metXY_60000, lt60000[i]) + 60001);
                        if (locnum > 0)	// If the location is allowed (if found, >0)
                        {
                            cbItem ncbi = new cbItem();
                            ncbi.Text = lt60000[i];
                            ncbi.Value = metlocs[locnum];
                            met_list.Add(ncbi);
                        }
                    }

                    CB_MetLocation.DataSource = met_list;
                    CB_MetLocation.DisplayMember = "Text";
                    CB_MetLocation.ValueMember = "Value";
                    CB_EggLocation.DataSource = new BindingSource(met_list, null);
                    CB_EggLocation.DisplayMember = "Text";
                    CB_EggLocation.ValueMember = "Value";
                    origintrack = "XY";
                    CB_EggLocation.SelectedValue = 0;
                    CB_MetLocation.SelectedValue = 0;
                }
                #endregion
            }
            if (gameorigin < 13 && gameorigin > 6 && origintrack != "Gen4")
            {   // Egg Met Locations for Gen 4 are unaltered when transferred to Gen 5. Need a new table if Gen 4 Origin.
                #region HGSS Met Locations
                // Allowed Met Locations
                int[] metlocs = { 0, 2000, 2002, 3001 };

                // Set up
                List<cbItem> met_list = new List<cbItem>();

                for (int i = 0; i < metlocs.Length; i++) // add entries to the top
                {
                    cbItem ncbi = new cbItem();
                    int locval = metlocs[i];
                    string loctext = "";

                    if (locval < 2000)
                        loctext = metHGSS_00000[locval];
                    else if (locval < 3000)
                        loctext = metHGSS_02000[locval % 2000];
                    else
                        loctext = metHGSS_03000[locval % 3000];

                    ncbi.Text = loctext;
                    ncbi.Value = locval;
                    met_list.Add(ncbi);
                }

                metlocs = new int[] 
                { 
                       0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,96,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,127,128,129,130,131,132,133,134,135,136,137,138,139,140,141,142,143,144,145,146,147,148,149,150,151,152,153,154,155,156,157,158,159,160,161,162,163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,
                       2000,2001,2002,2003,2004,2005,2006,2008,2009,2010,2011,2012,2013,2014,
                       3000,3001,3002,3003,3004,3005,3006,3007,3008,3009,3010,3011,3012,3013,3014,3015,3016,3017,3018,3019,3020,3021,3022,3023,3024,3025,3026,3027,3028,3029,3030,3031,3032,3033,3034,3035,3036,3037,3038,3039,3040,3041,3042,3043,3044,3045,3046,3047,3048,3049,3050,3051,3052,3053,3054,3055,3056,3057,3058,3059,3060,3061,3062,3063,3064,3065,3066,3067,3068,3069,3070,3071,3072,3073,3074,3075,3076
                };

                // Sort the Rest based on String Name
                string[] lt00000 = new string[metHGSS_00000.Length];
                string[] lt02000 = new string[metHGSS_02000.Length];
                string[] lt03000 = new string[metHGSS_03000.Length];
                Array.Copy(metHGSS_00000, lt00000, metHGSS_00000.Length);
                Array.Copy(metHGSS_02000, lt02000, metHGSS_02000.Length);
                Array.Copy(metHGSS_03000, lt03000, metHGSS_03000.Length);
                Array.Sort(lt00000);
                Array.Sort(lt02000);
                Array.Sort(lt03000);

                // Add the rest of the 0000
                for (int i = 0; i < lt00000.Length; i++)
                {
                    int locnum = Array.IndexOf(metlocs, Array.IndexOf(metHGSS_00000, lt00000[i]) + 0000);
                    if (locnum > 0)	// If the location is allowed (if found, >0)
                    {
                        cbItem ncbi = new cbItem();
                        ncbi.Text = lt00000[i];
                        ncbi.Value = metlocs[locnum];
                        met_list.Add(ncbi);
                    }
                }
                // Add the rest of the 2000
                for (int i = 0; i < lt02000.Length; i++)
                {
                    int locnum = Array.IndexOf(metlocs, Array.IndexOf(metHGSS_02000, lt02000[i]) + 2000);
                    if (locnum > 0)	// If the location is allowed (if found, >0)
                    {
                        cbItem ncbi = new cbItem();
                        ncbi.Text = lt02000[i];
                        ncbi.Value = metlocs[locnum];
                        met_list.Add(ncbi);
                    }
                }
                // Add the rest of the 3000
                for (int i = 0; i < lt03000.Length; i++)
                {
                    int locnum = Array.IndexOf(metlocs, Array.IndexOf(metHGSS_03000, lt03000[i]) + 3000);
                    if (locnum > 0)	// If the location is allowed (if found, >0)
                    {
                        cbItem ncbi = new cbItem();
                        ncbi.Text = lt03000[i];
                        ncbi.Value = metlocs[locnum];
                        met_list.Add(ncbi);
                    }
                }

                CB_EggLocation.DataSource = met_list;
                origintrack = "Gen4";
                CB_EggLocation.DisplayMember = "Text";
                CB_EggLocation.ValueMember = "Value";
                CB_EggLocation.SelectedValue = 0;
                #endregion
            }
        }
        private void updateExtraByteValue(object sender, EventArgs e)
        {
            // Changed Extra Byte's Value
            if (Util.ToInt32((sender as MaskedTextBox).Text) > 255)
                (sender as MaskedTextBox).Text = "255";

            int value = Util.ToInt32(TB_ExtraByte.Text);
            int offset = Convert.ToInt32(CB_ExtraBytes.Text, 16);
            buff[offset] = (byte)value;
        }
        private void updateExtraByteIndex(object sender, EventArgs e)
        {
            // Byte changed, need to refresh the Text box for the byte's value.
            TB_ExtraByte.Text = buff[Convert.ToInt32(CB_ExtraBytes.Text, 16)].ToString();
        }
        private void updateNickname(object sender, EventArgs e)
        {
            if (!CHK_Nicknamed.Checked)
            {
                // Fetch Current Species and set it as Nickname Text
                int species = Util.getIndex(CB_Species);
                if (species == 0 || species > 721)
                    TB_Nickname.Text = "";
                else
                {
                    // get language
                    int lang = Util.getIndex(CB_Language);
                    string[] lang_val = { "en", "ja", "fr", "it", "de", "es", "ko" };

                    string l = "";
                    switch (lang)
                    {
                        case 1: l = "ja"; break;
                        case 2: l = "en"; break;
                        case 3: l = "fr"; break;
                        case 4: l = "it"; break;
                        case 5: l = "de"; break;
                        case 7: l = "es"; break;
                        case 8: l = "ko"; break;
                        default: l = curlanguage; break;
                    }                    
                    TB_Nickname.Text = Util.getStringList("Species", l)[species];
                }
            }
        }
        private void updateNotOT(object sender, EventArgs e)
        {
            if (TB_OTt2.Text == "")
            {
                Label_CTGender.Text = "";
                TB_Friendship.Text = buff[0xCA].ToString();
                GB_OT.BackColor = System.Drawing.Color.FromArgb(232, 255, 255);
                GB_nOT.BackColor = Color.Transparent;
                buff[0x93] = 0;
            }
            else if (Label_CTGender.Text == "")
                Label_CTGender.Text = gendersymbols[0];
        }
        private void updatePKRSCured(object sender, EventArgs e)
        {
            // Cured PokeRus is toggled
            if (CHK_Cured.Checked)
            {
                // Has Had PokeRus
                CB_PKRSDays.Enabled = false;
                CB_PKRSDays.SelectedIndex = 0;

                CB_PKRSStrain.Enabled = true;
                CHK_Infected.Checked = true;

                if (CB_PKRSStrain.SelectedIndex == 0)
                {   // Give it Strain 1 by Default
                    CB_PKRSStrain.SelectedIndex = 1;
                }
            }
            else if (!CHK_Infected.Checked)
            {
                // Not Infected, Disable the other
                CB_PKRSStrain.Enabled = false;
                CB_PKRSStrain.SelectedIndex = 0;
            }
            else
            { // Still Infected for a duration
                CB_PKRSDays.Enabled = true;
                CB_PKRSDays.SelectedValue = 1;
            }
            // if not cured yet, days > 0
            if (!CHK_Cured.Checked && CHK_Infected.Checked && CB_PKRSDays.SelectedIndex == 0) 
                CB_PKRSDays.SelectedIndex++;
        
            setMarkings();
        }
        private void updatePKRSInfected(object sender, EventArgs e)
        {
            CB_PKRSDays.Enabled = CB_PKRSStrain.Enabled = CHK_Infected.Checked;
            if (!CHK_Infected.Checked) { CB_PKRSStrain.SelectedIndex = 0; CB_PKRSDays.SelectedIndex = 0; }
            else if (CB_PKRSStrain.SelectedIndex == 0) CB_PKRSStrain.SelectedIndex++;

            CB_PKRSDays.SelectedValue = CB_PKRSStrain.SelectedValue = Convert.ToInt32(CHK_Infected.Checked);
            // if not cured yet, days > 0
            if (!CHK_Cured.Checked && CHK_Infected.Checked && CB_PKRSDays.SelectedIndex == 0) CB_PKRSDays.SelectedIndex++;
        }
        private void updateIsEgg(object sender, EventArgs e)
        {
            if (CHK_IsEgg.Checked)
            {
                CHK_Nicknamed.Checked = false;
                TB_Nickname.Text = eggname;
                TB_Friendship.Text = "1";

                // If we are an egg, it won't have a met location.
                CHK_AsEgg.Checked = true;
                GB_EggConditions.Enabled = true;

                CAL_MetDate.Value = new DateTime(2000, 01, 01);
                CB_MetLocation.SelectedIndex = 2;
            }
            else // Not Egg
            {
                if (!CHK_Nicknamed.Checked)
                    updateNickname(null, null);

                TB_Friendship.Text = PKX.getBaseFriendship(Util.getIndex(CB_Species)).ToString();
                
                if (CB_EggLocation.SelectedIndex == 0)
                {
                    CAL_EggDate.Value = new DateTime(2000, 01, 01);
                    CHK_AsEgg.Checked = false;
                    GB_EggConditions.Enabled = false;
                }
            }
            // Display hatch counter if it is an egg, Display Friendship if it is not.
            Label_HatchCounter.Visible = CHK_IsEgg.Checked;
            Label_Friendship.Visible = !CHK_IsEgg.Checked;
        }
        private void updateMetAsEgg(object sender, EventArgs e)
        {
            GB_EggConditions.Enabled = CHK_AsEgg.Checked;
            if (!CHK_AsEgg.Checked)
            {
                // Remove egg met data
                CHK_IsEgg.Checked = false;
                CAL_EggDate.Value = new DateTime(2000, 01, 01);
                CB_EggLocation.SelectedValue = 0;
            }
        }
        private void updateShinyPID(object sender, EventArgs e)
        {
            uint PID = Util.getHEXval(TB_PID);
            uint UID = (PID >> 16);
            uint LID = (PID & 0xFFFF);
            uint PSV = UID ^ LID;
            uint TID = Util.ToUInt32(TB_TID);
            uint SID = Util.ToUInt32(TB_SID);
            uint TSV = TID ^ SID;
            uint XOR = TSV ^ PSV;

            // Check to see if we actually did it right... should be 0.
            if (XOR > 16)
                TB_PID.Text = ((UID ^ XOR) * 0x10000 + LID).ToString("X8");

            setIsShiny();
        }
        private void update_ID(object sender, EventArgs e)
        {
            // Trim out nonhex characters
            TB_PID.Text = Util.getHEXval(TB_PID).ToString("X8");
            TB_EC.Text = Util.getHEXval(TB_EC).ToString("X8");

            TB_TID.Text = Math.Min(Util.ToUInt32(TB_TID.Text), 65535).ToString(); // max TID/SID is 65535
            TB_SID.Text = Math.Min(Util.ToUInt32(TB_SID.Text), 65535).ToString();
            
            setIsShiny();
            updateIVs(null, null);   // If the PID is changed, PID%6 (Characteristic) might be changed. 
            TB_PID.Select(60, 0);   // position cursor at end of field
        }
        private void validateComboBox(object sender, CancelEventArgs e)
        {
            if (!(sender is ComboBox)) { return; }

            ComboBox cb = sender as ComboBox;
            cb.SelectionLength = 0;

            if ((cb.SelectedValue == null))
                cb.BackColor = Color.DarkSalmon;
            else
                cb.BackColor = Color.Empty;
        }
        private void validateComboBox2(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            validateComboBox(sender, e as CancelEventArgs);
            if (cb == CB_Ability)
                TB_AbilityNumber.Text = (1 << CB_Ability.SelectedIndex).ToString();
            else if ((cb == CB_Move1) || (cb == CB_Move2) || (cb == CB_Move3) || (cb == CB_Move4))
                updatePP(sender, e);

            updateIVs(null, null); // updating Nature will trigger stats to update as well
        }
        private void removedropCB(object sender, KeyEventArgs e)
        {
            ((ComboBox)sender).DroppedDown = false;
        }
        private void updateStats()
        {
            // Gather the needed information.
            species = Util.getIndex(CB_Species);
            int level = Util.ToInt32(TB_Level.Text); if (MT_Level.Enabled) level = Util.ToInt32(MT_Level.Text);
            if (level == 0) { level = 1; }
            int form = CB_Form.SelectedIndex;
            int HP_IV = Util.ToInt32(TB_HPIV.Text);
            int ATK_IV = Util.ToInt32(TB_ATKIV.Text);
            int DEF_IV = Util.ToInt32(TB_DEFIV.Text);
            int SPA_IV = Util.ToInt32(TB_SPAIV.Text);
            int SPD_IV = Util.ToInt32(TB_SPDIV.Text);
            int SPE_IV = Util.ToInt32(TB_SPEIV.Text);

            int HP_EV = Util.ToInt32(TB_HPEV.Text);
            int ATK_EV = Util.ToInt32(TB_ATKEV.Text);
            int DEF_EV = Util.ToInt32(TB_DEFEV.Text);
            int SPA_EV = Util.ToInt32(TB_SPAEV.Text);
            int SPD_EV = Util.ToInt32(TB_SPDEV.Text);
            int SPE_EV = Util.ToInt32(TB_SPEEV.Text);
            
            int nature = Util.getIndex(CB_Nature);

            // Generate the stats.
            ushort[] stats = PKX.getStats(species, level, nature, form,
                                        HP_EV, ATK_EV, DEF_EV, SPA_EV, SPD_EV, SPE_EV,
                                        HP_IV, ATK_IV, DEF_IV, SPA_IV, SPD_IV, SPE_IV);

            Stat_HP.Text  = stats[0].ToString();
            Stat_ATK.Text = stats[1].ToString();
            Stat_DEF.Text = stats[2].ToString();
            Stat_SPA.Text = stats[4].ToString();
            Stat_SPD.Text = stats[5].ToString();
            Stat_SPE.Text = stats[3].ToString();
            
            // Recolor the Stat Labels based on boosted stats.
            int incr = nature / 5;
            int decr = nature % 5;

            Label[] labarray = new Label[] { Label_ATK, Label_DEF, Label_SPE, Label_SPA, Label_SPD };
            for (int i = 0; i < 5; i++)
                labarray[i].ForeColor = DefaultForeColor;

            if (incr != decr) // if stat isn't neutral
            {
                labarray[incr].ForeColor = Color.Red;
                labarray[decr].ForeColor = Color.Blue;
            }
        }
        public void updateAbilityList(MaskedTextBox tb_abil, int species, ComboBox cb_abil, ComboBox cb_forme)
        {
            if (!init)
                return;
            int newabil = Convert.ToInt16(tb_abil.Text) >> 1;

            int form = cb_forme.SelectedIndex;
            int[] abils = PKX.getAbilities(species, form);                

            // Build Ability List
            List<string> ability_list = new List<string>();
            ability_list.Add(abilitylist[abils[0]] + " (1)");
            ability_list.Add(abilitylist[abils[1]] + " (2)");
            ability_list.Add(abilitylist[abils[2]] + " (H)");
            cb_abil.DataSource = ability_list;

            if (newabil < 3) cb_abil.SelectedIndex = newabil;
            else cb_abil.SelectedIndex = 0;
        }
        // Main Menu Strip UI Functions // 
        private void mainmenuOpen(object sender, EventArgs e)
        {
            string cyberpath = Util.GetTempFolder();
            SDFLoc = Util.GetSDFLocation();
            if (SDFLoc != null)
            {
                OpenPKX.InitialDirectory = SDFLoc;
                OpenPKX.RestoreDirectory = true;
                OpenPKX.FilterIndex = 4;
            }
            else if (Directory.Exists(Path.Combine(cyberpath, "root")))
            {
                OpenPKX.InitialDirectory = Path.Combine(cyberpath, "root");
                OpenPKX.RestoreDirectory = true;
                OpenPKX.FilterIndex = 4;
            }
            else if (Directory.Exists(cyberpath))
            {
                OpenPKX.InitialDirectory = cyberpath;
                OpenPKX.RestoreDirectory = true;
                OpenPKX.FilterIndex = 4;
            }
        
            DialogResult result = OpenPKX.ShowDialog();
            if (result == DialogResult.OK)
            {
                string path = OpenPKX.FileName;
                openQuick(path);            
            }
        }
        private void mainmenuSave(object sender, EventArgs e)
        {
            if (!verifiedpkx()) { return; }
            SavePKX.FileName = TB_Nickname.Text + " - " + TB_PID.Text;
            DialogResult result = SavePKX.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                string path = SavePKX.FileName;
                string ext = Path.GetExtension(path);

                if (File.Exists(path))
                {
                    // File already exists, save a .bak
                    byte[] backupfile = File.ReadAllBytes(path);
                    File.WriteAllBytes(path + ".bak", backupfile);
                }
                byte[] pkx = preparepkx(buff);

                if ((ext == ".ekx") || (ext == ".bin") || (ext == ".pkx") || (ext == ".ek6") || (ext == ".pk6"))
                {
                    if ((ext == ".ekx") || (ext == ".bin") || (ext == ".ek6")) // User Requested Encrypted File
                        pkx = PKX.encryptArray(pkx);
                    File.WriteAllBytes(path, pkx.ToArray());
                }
                else
                {
                    Util.Error("Foreign File Extension", "Exporting as encrypted.");
                    pkx = PKX.encryptArray(pkx);
                    File.WriteAllBytes(path, pkx);
                }
            }
        }
        private void mainmenuExit(object sender, EventArgs e)
        {
            this.Close();
        }
        private void mainmenuAbout(object sender, EventArgs e)
        {
            // Open a new form with the About details.
            new About().ShowDialog();
        }
        private void mainmenuWiden(object sender, EventArgs e)
        {
            int newwidth;
            if (Width < Height)
            {
                newwidth = largeWidth;
                tabBoxMulti.Enabled = true;
                tabBoxMulti.SelectedIndex = 0;
            }
            else
                newwidth = shortWidth;

            Width = newwidth;
        }
        private void mainmenuCodeGen(object sender, EventArgs e)
        {
            // Open Code Generator
            CodeGenerator CodeGen = new PKHeX.CodeGenerator(this);
            CodeGen.ShowDialog();
            byte[] data = CodeGen.returnArray;
            if (data != null)
            {
                byte[] decdata = PKX.decryptArray(data);
                Array.Copy(decdata, buff, 232);
                try { populateFields(buff); }
                catch
                {
                    Array.Copy(new Byte[232], buff, 232);
                    populateFields(buff);
                    Util.Error("Imported code did not decrypt properly", "Please verify that what you imported was correct.");
                }
            }
        }
        private void mainmenuBoxReport(object sender, EventArgs e)
        {
            frmReport ReportForm = new frmReport();
            int offset = 0x27A00; if (savegame_oras) offset = 0x33000 + 0x5400;
            ReportForm.PopulateData(savefile, savindex, offset);
            ReportForm.ShowDialog();
        }
        // Main Menu Subfunctions // 
        private void openQuick(string path)
        {
            // detect if it is a folder (load into boxes or not)
            if (Directory.Exists(path))
            { loadBoxesFromDB(path); return; }

            string ext = Path.GetExtension(path);
            FileInfo fi = new FileInfo(path);
            if (fi.Length > 0x10009C)
                Util.Error("Input file is too large.", path);
            else
            {

                byte[] input; try { input = File.ReadAllBytes(path); }
                catch { Util.Error("File is in use by another program!", path); return; } 

                try { openFile(input, path, ext); }
                catch
                {
                    try
                    {
                        byte[] blank = PKX.encryptArray(new Byte[260]);

                        for (int i = 0; i < 232; i++)
                            blank[i] = (byte)(blank[i] ^ input[i]);

                        openFile(blank, path, ext);
                    }
                    catch { openFile(input, path, ext); }
                }
            }
        }
        private void openFile(byte[] input, string path, string ext)
        {
            #region Powersaves Read-Only Conversion
            if (input.Length == 0x10009C) // Resize to 1MB
            {
                Array.Copy(input, 0x9C, input, 0, 0x100000); 
                Array.Resize(ref input, 0x100000);
            }
            #endregion
            #region Trade Packets
            if (input.Length == 363 && BitConverter.ToUInt16(input, 0x6B) == 0)
            {
                // EAD Packet of 363 length
                byte[] c = new Byte[260];
                Array.Copy(input, 0x67, c, 0, 260);
                input = c;
            }
            else if (input.Length == 407 && BitConverter.ToUInt16(input,0x98) == 0)
            {
                // EAD Packet of 407 length
                byte[] c = new Byte[260];
                Array.Copy(input, 0x93, c, 0, 260);
                input = c;
            }
            #endregion
            #region Saves
            else if ((input.Length == 0x76000) && BitConverter.ToUInt32(input, 0x75E10) == 0x42454546) // ORAS
                openMAIN(input, path, "ORAS", true);
            else if ((input.Length == 0x65600) && BitConverter.ToUInt32(input, 0x65410) == 0x42454546) // XY
                openMAIN(input, path, "XY", false);
            // Verify the Data Input Size is Proper
            else if (input.Length == 0x100000)
            {
                B_ExportSAV.Enabled = false;
                B_SwitchSAV.Enabled = false;
                B_JPEG.Enabled = false;
                string GameType = "XY"; // Default Game Type to load.
                if (BitConverter.ToUInt32(input, 0x7B210) == 0x42454546) GameType = "ORAS"; // BEEF magic in checksum block
                if ((BitConverter.ToUInt32(input, 0x100) != 0x41534944) && (BitConverter.ToUInt32(input, 0x5234) != 0x6E69616D))
                {
                    DialogResult dialogResult = Util.Prompt(MessageBoxButtons.YesNo, "Save file is not decrypted.", "Press Yes to ignore this warning and continue loading the save file.");
                    if (dialogResult == DialogResult.Yes)
                    {
                        DialogResult sdr = Util.Prompt(MessageBoxButtons.YesNoCancel, "Press Yes to load the sav at 0x3000", "Press No for the one at 0x82000");
                        if (sdr == DialogResult.Cancel)
                            return;
                        else if (sdr == DialogResult.Yes)
                        {
                            savindex = 0;
                            B_SwitchSAV.Enabled = true;
                        }
                        else savindex = 1;
                        B_SwitchSAV.Enabled = true;
                        open1MB(input, path, GameType, false);
                    }
                }
                else if (PKX.detectSAVIndex(input, ref savindex) == 2)
                {
                    DialogResult dialogResult = Util.Prompt(MessageBoxButtons.YesNo, "Hash verification failed.", "Press Yes to ignore this warning and continue loading the save file.");
                    if (dialogResult == DialogResult.Yes)
                    {
                        DialogResult sdr = Util.Prompt(MessageBoxButtons.YesNoCancel, "Press Yes to load the sav at 0x3000", "Press No for the one at 0x82000");
                        if (sdr == DialogResult.Cancel)
                        {
                            savindex = 0;
                            return; // abort load
                        }
                        else if (sdr == DialogResult.Yes)
                        {
                            savindex = 0;
                            B_SwitchSAV.Enabled = true;
                        }
                        else savindex = 1;
                        B_SwitchSAV.Enabled = true;
                        open1MB(input, path, GameType, false);
                    }
                }
                else
                {
                    B_ExportSAV.Enabled = true;
                    B_SwitchSAV.Enabled = true;
                    PKX.detectSAVIndex(input, ref savindex);
                    open1MB(input, path, GameType, false);
                }
                B_JPEG.Enabled = true;
            }
            #endregion
            #region PK6/EK6
            else if ((input.Length == 260) || (input.Length == 232))
            {
                // Check if Input is PKX
                if ((ext == ".pk6") || (ext == ".ek6") || (ext == ".pkx") || (ext == ".ekx") || (ext == ".bin") || (ext == ""))
                {
                    // Check if either is encrypted from another program.
                    if (BitConverter.ToUInt16(input, 0xC8) == 0 && BitConverter.ToUInt16(input, 0x58) == 0)
                    {   // File isn't encrypted.
                        buff = input;
                        populateFields(buff);
                    }
                    else
                    {
                        // File is encrypted.
                        buff = PKX.decryptArray(input);
                        populateFields(buff);
                    }
                }
                else
                    Util.Error("Unable to recognize file.", "Only valid .pk* .ek* .bin supported.");
            }
            #endregion
            #region PK3/PK4/PK5
            else if ((input.Length == 136) || (input.Length == 220) || (input.Length == 236) || (input.Length == 100) || (input.Length == 80)) // to convert g5pkm
            {
                var Converter = new pk2pk();
                if (!PKX.verifychk(input)) Util.Error("Invalid File (Checksum Error)");
                try // to convert g5pkm
                {
                    byte[] data = Converter.ConvertPKM(input, savefile, savindex);
                    Array.Copy(data, buff, 232);
                    populateFields(buff);
                }
                catch
                {
                    Array.Copy(new Byte[232], buff, 232);
                    populateFields(buff);
                    Util.Error("Attempted to load previous generation PKM.", "Conversion failed.");
                }
            }
            #endregion
            else
                Util.Error("Attempted to load an unsupported file type/size.", "File Loaded:" + Environment.NewLine + path);
        }
        private void openMAIN(byte[] input, string path, string GameType, bool oras)
        {
            L_Save.Text = "SAV: " + Path.GetFileName(path);
            SaveGame = new SaveGames.SaveStruct(GameType);

            // Load CyberGadget
            this.savindex = 0;
            this.savefile = new Byte[0x100000];
            this.cyberSAV = input;
            cybergadget = true;
            B_ExportSAV.Enabled = true;
            Array.Copy(input, 0, savefile, 0x5400, input.Length);

            openSave(oras);
        }
        private void open1MB(byte[] input, string path, string GameType, bool oras)
        {
            L_Save.Text = "SAV: " + Path.GetFileName(path);
            SaveGame = new SaveGames.SaveStruct(GameType);
            savegame_oras = oras;

            savefile = input;
            cybergadget = false;            

            // Logic to allow unlocking of Switch SAV
            // Setup SHA
            SHA256 mySHA256 = SHA256Managed.Create();

            // Check both IVFC Hashes
            byte[] zeroarray = new Byte[0x200];
            Array.Copy(savefile, 0x2000 + 0 * 0x7F000, zeroarray, 0, 0x20);
            byte[] hashValue1 = mySHA256.ComputeHash(zeroarray);
            Array.Copy(savefile, 0x2000 + 1 * 0x7F000, zeroarray, 0, 0x20);
            byte[] hashValue2 = mySHA256.ComputeHash(zeroarray);

            byte[] realHash1 = new Byte[0x20];
            byte[] realHash2 = new Byte[0x20];

            Array.Copy(savefile, 0x43C - 0 * 0x130, realHash1, 0, 0x20);
            Array.Copy(savefile, 0x43C - 1 * 0x130, realHash2, 0, 0x20);

            B_SwitchSAV.Enabled = (hashValue1.SequenceEqual(realHash1) && hashValue2.SequenceEqual(realHash2));
            getSAVOffsets(); // just in case
            Array.Copy(savefile, 0x5400 + 0x7F000 * savindex, cyberSAV, 0, cyberSAV.Length);

            openSave(oras);
        }
        private void openSave(bool oras)
        {
            savegame_oras = oras;
            // Enable Secondary Tools
            GB_SAVtools.Enabled =
                tabBoxMulti.Enabled = true;
                B_JPEG.Enabled =
                B_BoxIO.Enabled =
                B_VerifyCHK.Enabled = true;
            B_VerifySHA.Enabled = B_SwitchSAV.Enabled = false;

            savedited = false;
            Menu_ToggleBoxUI.Visible = false;

            // Set up Boxes
            C_BoxSelect.SelectedIndex = 0;
            tabBoxMulti.SelectedIndex = 0;

            setBoxNames();   // Display the Box Names
            setPKXBoxes();   // Reload all of the PKX Windows
            setSAVLabel();   // Reload the label indicating current save

            // Version Exclusive Editors
            GB_SUBE.Visible = !oras;
            B_OpenSecretBase.Visible = oras;

            this.Width = largeWidth;
            savLoaded = true;
            System.Media.SystemSounds.Beep.Play(); // audibly indicate the save is loaded
        }
        // Secondary Windows for Ribbons/Amie/Memories
        private void openribbons(object sender, EventArgs e)
        {
            new RibbMedal(this).ShowDialog();
        }
        private void openhistory(object sender, EventArgs e)
        {
            new MemoryAmie(this).ShowDialog();
        }
        // Open/Save Array Manipulation // 
        public bool verifiedpkx()
        {
            if (ModifierKeys == (Keys.Control | Keys.Shift | Keys.Alt))
                return true; // Override
            // Make sure the PKX Fields are filled out properly (color check)
            #region ComboBoxes
            ComboBox[] cba = {
                                 CB_Species, CB_Nature, CB_HeldItem, CB_Ability, CB_Country, CB_SubRegion, // Main Tab
                                 CB_MetLocation, CB_EggLocation, CB_Ball,   // Met Tab
                                 CB_Move1, CB_Move2, CB_Move3, CB_Move4,    // Moves
                                 CB_RelearnMove1, CB_RelearnMove2, CB_RelearnMove3, CB_RelearnMove4 // Moves
                             };
            for (int i = 0; i < cba.Length; i++)
            {
                int back = cba[i].BackColor.ToArgb();
                if (back != System.Drawing.SystemColors.Control.ToArgb() && back != 0 && back != -1)
                {
                    if (i < 6)      // Main Tab
                        tabMain.SelectedIndex = 0;
                    else if (i < 9) // Met Tab
                        tabMain.SelectedIndex = 1;
                    else            // Moves
                        tabMain.SelectedIndex = 3;
                    goto invalid;
                }
            }
            #endregion
            // Further logic checking
            if (Convert.ToUInt32(TB_EVTotal.Text) > 510 && !CHK_HackedStats.Checked)
            { tabMain.SelectedIndex = 2; goto invalid; }
            if (Util.getIndex(CB_Species) == 0) // Not gonna write 0 species.
            { tabMain.SelectedIndex = 0; goto invalid; }

            // If no errors detected...
            return true;
            // else...
          invalid:
            { System.Media.SystemSounds.Exclamation.Play(); return false; }
        }
        public byte[] preparepkx(byte[] buff)
        {
            tabMain.Select(); // hack to make sure comboboxes are set (users scrolling through and immediately setting causes this)
            // Stuff the global byte array with our PKX form data
            // Create a new storage so we don't muck up things with the original
            byte[] pkx = new byte[0x104];
            Array.Copy(buff, pkx, 0x104);
            // Repopulate PKX with Edited Stuff
            if (Util.getIndex(CB_GameOrigin) < 24)
            {
                uint EC = Util.getHEXval(TB_EC);
                uint PID = Util.getHEXval(TB_PID);
                uint SID = Util.ToUInt32(TB_TID.Text);
                uint TID = Util.ToUInt32(TB_TID.Text);
                uint LID = PID & 0xFFFF;
                uint HID = PID >> 16;
                uint XOR = (TID ^ LID ^ SID ^ HID);

                // Ensure we don't have a shiny.
                if (XOR < 16 && XOR >= 8) // Illegal, fix.
                {
                    // Keep as shiny, so we have to mod the PID
                    PID = (PID ^ XOR);
                    TB_PID.Text = PID.ToString("X8");
                    TB_EC.Text = PID.ToString("X8");
                }
                else if ((XOR ^ 0x8000) >= 8 && (XOR ^ 0x8000) < 16 && (PID != EC))
                    TB_EC.Text = (PID ^ 0x80000000).ToString("X8");
                else // Not Illegal, no fix.
                    TB_EC.Text = PID.ToString("X8");
            }

            Array.Copy(BitConverter.GetBytes(Util.getHEXval(TB_EC)), 0, pkx, 0, 4);  // EC
            Array.Copy(BitConverter.GetBytes(0), 0, pkx, 0x4, 4);  // 0 CHK for now

            // Block A
            int species = Util.getIndex(CB_Species);
            Array.Copy(BitConverter.GetBytes(species), 0, pkx, 0x08, 2);      // Species
            Array.Copy(BitConverter.GetBytes(Util.getIndex(CB_HeldItem)), 0, pkx, 0x0A, 2);     // Held Item
            Array.Copy(BitConverter.GetBytes(Util.ToUInt32(TB_TID.Text)), 0, pkx, 0x0C, 2);     // TID
            Array.Copy(BitConverter.GetBytes(Util.ToUInt32(TB_SID.Text)), 0, pkx, 0x0E, 2);     // SID
            Array.Copy(BitConverter.GetBytes(Convert.ToUInt32(TB_EXP.Text)), 0, pkx, 0x10, 4);  // EXP
            pkx[0x14] = (byte)Array.IndexOf(abilitylist, (CB_Ability.Text).Remove((CB_Ability.Text).Length - 4)); // Ability
            pkx[0x15] = (byte)(Util.ToInt32((TB_AbilityNumber.Text)));   // Number
            // pkx[0x16], pkx[0x17] are handled by the Medals UI (Hits & Training Bag)
            Array.Copy(BitConverter.GetBytes(Util.getHEXval(TB_PID)), 0, pkx, 0x18, 4);         // PID
            pkx[0x1C] = (byte)((Util.getIndex(CB_Nature)));                                     // Nature
            int fegform = (int)(Convert.ToInt32(CHK_Fateful.Checked));                          // Fateful
            fegform |= (PKX.getGender(Label_Gender.Text) << 1);                                 // Gender
            if (MT_Form.Enabled) fegform |= (Math.Min(Convert.ToInt32(MT_Form.Text), 32) << 3); // Form
            else fegform |= ((Util.getIndex(CB_Form)) << 3); 
            pkx[0x1D] = (byte)fegform;
            pkx[0x1E] = (byte)(Util.ToInt32(TB_HPEV.Text) & 0xFF);       // EVs
            pkx[0x1F] = (byte)(Util.ToInt32(TB_ATKEV.Text) & 0xFF);
            pkx[0x20] = (byte)(Util.ToInt32(TB_DEFEV.Text) & 0xFF);
            pkx[0x21] = (byte)(Util.ToInt32(TB_SPEEV.Text) & 0xFF);
            pkx[0x22] = (byte)(Util.ToInt32(TB_SPAEV.Text) & 0xFF);
            pkx[0x23] = (byte)(Util.ToInt32(TB_SPDEV.Text) & 0xFF);

            pkx[0x24] = (byte)(Util.ToInt32(TB_Cool.Text) & 0xFF);       // CNT
            pkx[0x25] = (byte)(Util.ToInt32(TB_Beauty.Text) & 0xFF);
            pkx[0x26] = (byte)(Util.ToInt32(TB_Cute.Text) & 0xFF);
            pkx[0x27] = (byte)(Util.ToInt32(TB_Smart.Text) & 0xFF);
            pkx[0x28] = (byte)(Util.ToInt32(TB_Tough.Text) & 0xFF);
            pkx[0x29] = (byte)(Util.ToInt32(TB_Sheen.Text) & 0xFF);
            // stupid & 0xFF to prevent bad values being stuffed
            int markings = Convert.ToInt32(CHK_Circle.Checked);
            markings += Convert.ToInt32(CHK_Triangle.Checked) * 2;
            markings += Convert.ToInt32(CHK_Square.Checked) * 4;
            markings += Convert.ToInt32(CHK_Heart.Checked) * 8;
            markings += Convert.ToInt32(CHK_Star.Checked) * 16;
            markings += Convert.ToInt32(CHK_Diamond.Checked) * 32;
            pkx[0x2A] = (byte)markings;
            pkx[0x2B] = (byte)(CB_PKRSDays.SelectedIndex + CB_PKRSStrain.SelectedIndex * 0x10);

            // Already in buff (then transferred to new pkx)
            // 0x2C, 0x2D, 0x2E, 0x2F
            // 0x30, 0x31, 0x32, 0x33
            // 0x34, 0x35, 0x36, 0x37
            // 0x38, 0x39

            // Unused
            // 0x3A, 0x3B
            // 0x3C, 0x3D, 0x3E, 0x3F

            // Block B
            // Convert Nickname field back to bytes
            string nicknamestr = TB_Nickname.Text;

            if (!CHK_Nicknamed.Checked) // Get the correct gender sign
            {
                if (nicknamestr.Contains((char)0x2640))
                    nicknamestr = Regex.Replace(nicknamestr, "\u2640", "\uE08F");
                else if (nicknamestr.Contains((char)0x2642))
                    nicknamestr = Regex.Replace(nicknamestr, "\u2642", "\uE08E");
            }
            byte[] nicknamebytes = Encoding.Unicode.GetBytes(nicknamestr);
            Array.Resize(ref nicknamebytes, 24); // pad with zeroes and effectively keep no trash bytes
            Array.Copy(nicknamebytes, 0, pkx, 0x40, nicknamebytes.Length);

            // 0x58, 0x59 unused
            Array.Copy(BitConverter.GetBytes(Util.getIndex(CB_Move1)), 0, pkx, 0x5A, 2);  // Move 1
            Array.Copy(BitConverter.GetBytes(Util.getIndex(CB_Move2)), 0, pkx, 0x5C, 2);  // Move 2
            Array.Copy(BitConverter.GetBytes(Util.getIndex(CB_Move3)), 0, pkx, 0x5E, 2);  // Move 3
            Array.Copy(BitConverter.GetBytes(Util.getIndex(CB_Move4)), 0, pkx, 0x60, 2);  // Move 4

            pkx[0x62] = (byte)(Util.ToInt32(TB_PP1.Text) & 0xFF);    // Max PP
            pkx[0x63] = (byte)(Util.ToInt32(TB_PP2.Text) & 0xFF);
            pkx[0x64] = (byte)(Util.ToInt32(TB_PP3.Text) & 0xFF);
            pkx[0x65] = (byte)(Util.ToInt32(TB_PP4.Text) & 0xFF);

            pkx[0x66] = (byte)(CB_PPu1.SelectedIndex);          // PP Ups
            pkx[0x67] = (byte)(CB_PPu2.SelectedIndex);
            pkx[0x68] = (byte)(CB_PPu3.SelectedIndex);
            pkx[0x69] = (byte)(CB_PPu4.SelectedIndex);

            // Don't allow PP Ups if there is no move.
            for (int i = 0; i < 4; i++)
                if (pkx[0x62 + i] == 0) pkx[0x66 + i] = 0;

            Array.Copy(BitConverter.GetBytes(Util.getIndex(CB_RelearnMove1)), 0, pkx, 0x6A, 2);  // EggMove 1
            Array.Copy(BitConverter.GetBytes(Util.getIndex(CB_RelearnMove2)), 0, pkx, 0x6C, 2);  // EggMove 2
            Array.Copy(BitConverter.GetBytes(Util.getIndex(CB_RelearnMove3)), 0, pkx, 0x6E, 2);  // EggMove 3
            Array.Copy(BitConverter.GetBytes(Util.getIndex(CB_RelearnMove4)), 0, pkx, 0x70, 2);  // EggMove 4

            // 0x72 - Ribbon editor sets this flag (Secret Super Training)
            // 0x73

            uint IV32 = Util.ToUInt32(TB_HPIV.Text) & 0x1F;
            IV32 += ((Util.ToUInt32(TB_ATKIV.Text) & 0x1F) << 5);
            IV32 += ((Util.ToUInt32(TB_DEFIV.Text) & 0x1F) << 10);
            IV32 += ((Util.ToUInt32(TB_SPEIV.Text) & 0x1F) << 15);
            IV32 += ((Util.ToUInt32(TB_SPAIV.Text) & 0x1F) << 20);
            IV32 += ((Util.ToUInt32(TB_SPDIV.Text) & 0x1F) << 25);
            IV32 += (Convert.ToUInt32(CHK_IsEgg.Checked) << 30);
            IV32 += (Convert.ToUInt32(CHK_Nicknamed.Checked) << 31);

            pkx[0x74] = (byte)((IV32 >> 00) & 0xFF); // IVs
            pkx[0x75] = (byte)((IV32 >> 08) & 0xFF);
            pkx[0x76] = (byte)((IV32 >> 16) & 0xFF);
            pkx[0x77] = (byte)((IV32 >> 24) & 0xFF);

            // Block C
            // Convert OTT2 field back to bytes
            byte[] OT2 = Encoding.Unicode.GetBytes(TB_OTt2.Text);
            Array.Resize(ref OT2, 24);
            Array.Copy(OT2, 0, pkx, 0x78, OT2.Length);

            //0x90-0xAF
            pkx[0x92] = Convert.ToByte(PKX.getGender(Label_CTGender.Text) == 1);
            //Plus more, set by MemoryAmie (already in buff)

            // Block D
            // Convert OT field back to bytes
            byte[] OT = Encoding.Unicode.GetBytes(TB_OT.Text);
            Array.Resize(ref OT, 24);
            Array.Copy(OT, 0, pkx, 0xB0, OT.Length);

            if (pkx[0x93] == 0)
                pkx[0xCA] = (byte)(Util.ToInt32(TB_Friendship.Text) & 0xFF);
            else          // 1
                pkx[0xA2] = (byte)(Util.ToInt32(TB_Friendship.Text) & 0xFF);

            int egg_year = 2000;                                   // Dates
            int egg_month = 0;
            int egg_day = 0;
            int egg_location = 0;
            if (CHK_AsEgg.Checked)      // If encountered as an egg, load the Egg Met data from fields.
            {
                egg_year = CAL_EggDate.Value.Year;
                egg_month = CAL_EggDate.Value.Month;
                egg_day = CAL_EggDate.Value.Day;
                egg_location = Util.getIndex(CB_EggLocation);
            }
            // Egg Met Data
            pkx[0xD1] = (byte)(egg_year - 2000);
            pkx[0xD2] = (byte)egg_month;
            pkx[0xD3] = (byte)egg_day;
            // Met Data
            pkx[0xD4] = (byte)(CAL_MetDate.Value.Year - 2000);
            pkx[0xD5] = (byte)CAL_MetDate.Value.Month;
            pkx[0xD6] = (byte)CAL_MetDate.Value.Day;

            if (CHK_IsEgg.Checked && CB_MetLocation.SelectedIndex == 0)    // If still an egg, it has no hatch location/date. Zero it!
            {
                pkx[0xD4] = 0;
                pkx[0xD5] = 0;
                pkx[0xD6] = 0;
            }

            // 0xD7 Unknown
            int met_location = Util.getIndex(CB_MetLocation);    // Locations
            Array.Copy(BitConverter.GetBytes(egg_location), 0, pkx, 0xD8, 2);   // Egg Location
            Array.Copy(BitConverter.GetBytes(met_location), 0, pkx, 0xDA, 2);   // Met Location

            pkx[0xDC] = (byte)Util.getIndex(CB_Ball);
            pkx[0xDD] = (byte)(((Util.ToInt32(TB_MetLevel.Text) & 0x7F) + (Convert.ToInt32(PKX.getGender(Label_OTGender.Text) == 1) << 7)));
            pkx[0xDE] = (byte)(Util.ToInt32(CB_EncounterType.SelectedValue.ToString()));
            pkx[0xDF] = (byte)Util.getIndex(CB_GameOrigin);
            pkx[0xE0] = (byte)Util.getIndex(CB_Country);
            pkx[0xE1] = (byte)Util.getIndex(CB_SubRegion);
            pkx[0xE2] = (byte)Util.getIndex(CB_3DSReg);
            pkx[0xE3] = (byte)Util.getIndex(CB_Language);
            // 0xE4-0xE7

            Array.Resize(ref pkx, 260);
            // Party Stats
            pkx[0xE8] = 0; pkx[0xE9] = 0;
            pkx[0xEA] = 0; pkx[0xEB] = 0;
            pkx[0xEC] = (byte)Util.ToInt32(TB_Level.Text);                                                     // Level
            pkx[0xED] = 0; pkx[0xEE] = 0; pkx[0xEF] = 0;
            Array.Copy(BitConverter.GetBytes(Math.Min(Util.ToInt32(Stat_HP.Text), 65535)), 0, pkx, 0xF0, 2);   // Current HP
            Array.Copy(BitConverter.GetBytes(Math.Min(Util.ToInt32(Stat_HP.Text), 65535)), 0, pkx, 0xF2, 2);   // Max HP
            Array.Copy(BitConverter.GetBytes(Math.Min(Util.ToInt32(Stat_ATK.Text), 65535)), 0, pkx, 0xF4, 2);  // ATK
            Array.Copy(BitConverter.GetBytes(Math.Min(Util.ToInt32(Stat_DEF.Text), 65535)), 0, pkx, 0xF6, 2);  // DEF
            Array.Copy(BitConverter.GetBytes(Math.Min(Util.ToInt32(Stat_SPE.Text), 65535)), 0, pkx, 0xF8, 2);  // SPE
            Array.Copy(BitConverter.GetBytes(Math.Min(Util.ToInt32(Stat_SPA.Text), 65535)), 0, pkx, 0xFA, 2);  // SPA
            Array.Copy(BitConverter.GetBytes(Math.Min(Util.ToInt32(Stat_SPD.Text), 65535)), 0, pkx, 0xFC, 2);  // SPD
            pkx[0xFE]  = 0; pkx[0xFF]  = 0;
            pkx[0x100] = 0; pkx[0x101] = 0; pkx[0x102] = 0; pkx[0x103] = 0;

            // Hax Illegality
            if (DEV_Ability.Enabled)
            {
                pkx[0x14] = (byte)Util.getIndex(DEV_Ability);                                                   // Ability
                pkx[0xEC] = (byte)Math.Min(Convert.ToInt32(MT_Level.Text), 255);                                // Level
            }

            // Fix Moves if a slot is empty
            for (int i = 0; i < 3; i++)
            {
                if (BitConverter.ToUInt16(pkx, 0x5A + 2 * i) == 0)
                {
                    Array.Copy(pkx, 0x5C + 2 * i, pkx, 0x5A + 2 * i, 2); // Shift moves down
                    Array.Copy(new Byte[2], 0, pkx, 0x5C + 2 * i, 2);   // Clear next move (error shifted down)

                    // Move PP and PP Ups down one byte.
                    pkx[0x62 + i] = pkx[0x63 + i]; pkx[0x63 + i] = 0; // PP
                    pkx[0x66 + i] = pkx[0x67 + i]; pkx[0x67 + i] = 0; // PP Ups
                }
            }

            // Fix Relearn moves if a slot is empty
            for (int i = 0; i < 3; i++)
            {
                if (BitConverter.ToUInt16(pkx, 0x6A + 2 * i) == 0)
                {
                    Array.Copy(pkx, 0x6C + 2 * i, pkx, 0x6A + 2 * i, 2); // Shift moves down
                    Array.Copy(new Byte[2], 0, pkx, 0x6C + 2 * i, 2);   // Clear next move (error shifted down)
                }
            }

            // No foreign memories for Pokemon without a foreign trainer
            if (BitConverter.ToUInt16(pkx, 0x78) == 0)
            {
                pkx[0xA2] = pkx[0xA3] =                 // No Friendship/Affection
                pkx[0xA8] = pkx[0xA9] =                 // No Memory Var
                pkx[0xA4] = pkx[0xA5] = pkx[0xA6] = 0;  // No Memory Types
            }

            // Now we fix the checksum!
            ushort chk = 0;
            for (int i = 8; i < 232; i += 2) // Loop through the entire PKX
                chk += BitConverter.ToUInt16(pkx, i);

            // Apply New Checksum
            Array.Copy(BitConverter.GetBytes(chk), 0, pkx, 06, 2);

            // PKX is now filled
            return pkx;
        }
        // Drag & Drop Events
        private void tabMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        private void tabMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string path = files[0]; // open first D&D
            openQuick(path);
        }
        // Decrypted Export
        private void dragout_MouseDown(object sender, MouseEventArgs e)
        {
            if (!verifiedpkx()) { return; }
            {
                // Create Temp File to Drag
                string basepath = System.Windows.Forms.Application.StartupPath;
                Cursor.Current = Cursors.Hand;
                // Make a new file name
                byte[] dragdata = preparepkx(buff);
                PKX pkx = new PKX(dragdata);
                string filename = pkx.Nickname;
                if (filename != pkx.Species)
                    filename += " (" + pkx.Species + ")";
                filename += " - " + pkx.PID + ".pk6";

                // Strip out party stats (if they are there)
                Array.Resize(ref dragdata, 232);
                // Make File
                string newfile = Path.Combine(basepath, Util.CleanFileName(filename));
                try
                {
                    File.WriteAllBytes(newfile, dragdata);

                    string[] filesToDrag = { newfile };
                    dragout.DoDragDrop(new DataObject(DataFormats.FileDrop, filesToDrag), DragDropEffects.Move);
                    File.Delete(newfile);
                }
                catch (Exception x)
                { Util.Error("Drag & Drop Error", x.ToString()); }
                File.Delete(newfile);
            }
        }
        private void dragout_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        // Encrypted Export
        private void eragout_MouseDown(object sender, MouseEventArgs e)
        {
            if (!verifiedpkx()) { return; }
            {
                // Create Temp File to Drag
                string basepath = System.Windows.Forms.Application.StartupPath;
                Cursor.Current = Cursors.Hand;

                // Make a new file name
                byte[] dragdata = preparepkx(buff);
                PKX pkx = new PKX(dragdata);
                string filename = pkx.Nickname;
                if (filename != pkx.Species)
                    filename += " (" + pkx.Species + ")";
                filename += " - " + pkx.PID + ".ek6";
                dragdata = PKX.encryptArray(preparepkx(buff));
                // Strip out party stats (if they are there)
                Array.Resize(ref dragdata, 232);
                // Make file
                string newfile = Path.Combine(basepath, Util.CleanFileName(filename));
                try
                {
                    File.WriteAllBytes(newfile, dragdata);

                    string[] filesToDrag = { newfile };
                    dragout.DoDragDrop(new DataObject(DataFormats.FileDrop, filesToDrag), DragDropEffects.Move);
                    File.Delete(newfile);
                }
                catch (Exception x)
                { Util.Error("Drag & Drop Error", x.ToString()); }
                File.Delete(newfile);
            }
        }
        private void eragout_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        // Dragout Hover Display
        private void dragoutHover(object sender, EventArgs e)
        {
            eragout.BackColor = dragout.BackColor = Color.LightGray;
        }
        private void dragoutLeave(object sender, EventArgs e)
        {
            eragout.BackColor = dragout.BackColor = Color.Transparent;
        }
        #endregion

        #region //// SAVE FILE FUNCTIONS ////
        // Integrity Checks // 
        private void B_VerifyCHK_Click(object sender, EventArgs e)
        {
            if (savedited)
            {
                RTB_S.Text = "Save has been edited. Cannot integrity check.";
                return;
            }
            // Verify Checksums
            RTB_S.Text = "";
            int invalid1 = 0;
            uint[] start = { 0x05400, 0x05800, 0x06400, 0x06600, 0x06800, 0x06A00, 0x06C00, 0x06E00, 0x07000, 0x07200, 0x07400, 0x09600, 0x09800, 0x09E00, 0x0A400, 0x0F400, 0x14400, 0x19400, 0x19600, 0x19E00, 0x1A400, 0x1AC00, 0x1B400, 0x1B600, 0x1B800, 0x1BE00, 0x1C000, 0x1C400, 0x1CC00, 0x1CE00, 0x1D000, 0x1D200, 0x1D400, 0x1D600, 0x1DE00, 0x1E400, 0x1E800, 0x20400, 0x20600, 0x20800, 0x20C00, 0x21000, 0x22C00, 0x23000, 0x23800, 0x23C00, 0x24600, 0x24A00, 0x25200, 0x26000, 0x26200, 0x26400, 0x27200, 0x27A00, 0x5C600, };
            uint[] length = { 0x000002C8,	0x00000B88,	0x0000002C,	0x00000038,	0x00000150,	0x00000004,	0x00000008,	0x000001C0,	0x000000BE,	0x00000024,	0x00002100,	0x00000140,	0x00000440,	0x00000574,	0x00004E28,	0x00004E28,	0x00004E28,	0x00000170,	0x0000061C,	0x00000504,	0x000006A0,	0x00000644,	0x00000104,	0x00000004,	0x00000420,	0x00000064,	0x000003F0,	0x0000070C,	0x00000180,	0x00000004,	0x0000000C,	0x00000048,	0x00000054,	0x00000644,	0x000005C8,	0x000002F8,	0x00001B40,	0x000001F4,	0x000001F0,	0x00000216,	0x00000390,	0x00001A90,	0x00000308,	0x00000618,	0x0000025C,	0x00000834,	0x00000318,	0x000007D0,	0x00000C48,	0x00000078,	0x00000200,	0x00000C84,	0x00000628,	0x00034AD0,	0x0000E058, };
            int csoff = 0x6A81A;

            if (savegame_oras)
            {
                start = new uint[] { 0x05400, 0x05800, 0x06400, 0x06600, 0x06800, 0x06A00, 0x06C00, 0x06E00, 0x07000, 0x07200, 0x07400, 0x09600, 0x09800, 0x09E00, 0x0A400, 0x0F400, 0x14400, 0x19400, 0x19600, 0x19E00, 0x1A400, 0x1B600, 0x1BE00, 0x1C000, 0x1C200, 0x1C800, 0x1CA00, 0x1CE00, 0x1D600, 0x1D800, 0x1DA00, 0x1DC00, 0x1DE00, 0x1E000, 0x1E800, 0x1EE00, 0x1F200, 0x20E00, 0x21000, 0x21400, 0x21800, 0x22000, 0x23C00, 0x24000, 0x24800, 0x24C00, 0x25600, 0x25A00, 0x26200, 0x27000, 0x27200, 0x27400, 0x28200, 0x28A00, 0x28E00, 0x30A00, 0x38400, 0x6D000, };
                length = new uint[] { 0x000002C8, 0x00000B90, 0x0000002C, 0x00000038, 0x00000150, 0x00000004, 0x00000008, 0x000001C0, 0x000000BE, 0x00000024, 0x00002100, 0x00000130, 0x00000440, 0x00000574, 0x00004E28, 0x00004E28, 0x00004E28, 0x00000170, 0x0000061C, 0x00000504, 0x000011CC, 0x00000644, 0x00000104, 0x00000004, 0x00000420, 0x00000064, 0x000003F0, 0x0000070C, 0x00000180, 0x00000004, 0x0000000C, 0x00000048, 0x00000054, 0x00000644, 0x000005C8, 0x000002F8, 0x00001B40, 0x000001F4, 0x000003E0, 0x00000216, 0x00000640, 0x00001A90, 0x00000400, 0x00000618, 0x0000025C, 0x00000834, 0x00000318, 0x000007D0, 0x00000C48, 0x00000078, 0x00000200, 0x00000C84, 0x00000628, 0x00000400, 0x00007AD0, 0x000078B0, 0x00034AD0, 0x0000E058, };
                csoff = 0x7B21A;
            }

            for (int i = 0; i < length.Length; i++)
            {
                byte[] data = new Byte[length[i]];
                Array.Copy(savefile, start[i], data, 0, length[i]);
                ushort checksum = PKX.ccitt16(data);
                ushort actualsum = BitConverter.ToUInt16(savefile, csoff + i * 0x8);
                if (checksum != actualsum)
                {
                    invalid1++;
                    RTB_S.Text += "Invalid: " + i.ToString("X2") + " @ region " + start[i].ToString("X5") + Environment.NewLine;
                }
            }
            RTB_S.Text += "1st SAV: " + (start.Length - invalid1).ToString() + "/" + start.Length.ToString() + Environment.NewLine;

            if (cybergadget) return;

            // Do it again, but for the second SAV.
            csoff += 0x7F000;

            for (int i = 0; i < start.Length; i++) // Shift all the save offsets...
                start[i] += 0x7F000;

            int invalid2 = 0;
            for (int i = 0; i < length.Length; i++)
            {
                byte[] data = new Byte[length[i]];
                Array.Copy(savefile, start[i], data, 0, length[i]);
                ushort checksum = PKX.ccitt16(data);
                ushort actualsum = BitConverter.ToUInt16(savefile,csoff + i * 0x8);
                if (checksum != actualsum)
                {
                    invalid2++;
                    RTB_S.Text += "Invalid: " + i.ToString("X2") + " @ region " + start[i].ToString("X5") + Environment.NewLine;
                }
            }
            RTB_S.Text += "2nd SAV: " + (start.Length - invalid2).ToString() + "/" + start.Length.ToString() + Environment.NewLine;
            if (invalid1 + invalid2 == (start.Length * 2))
                RTB_S.Text = "No checksums are valid.";
        }
        private void B_VerifySHA_Click(object sender, EventArgs e)
        {
            if (savedited)
            {
                RTB_S.Text = "Save has been edited. Cannot integrity check.";
                return;
            }
            // Verify Hashes
            RTB_S.Text = "";
            int invalid1 = 0;
            #region hash table data
            uint[] hashtabledata = {
                                    0x2020,	    0x203F,	    0x2000,	0x200,
                                    0x2040,	    0x2FFF,	    0x2020,	0x1000,
                                    0x3000,	    0x3FFF,	    0x2040,	0x1000,
                                    0x4000,	    0x4FFF,	    0x2060,	0x1000,
                                    0x5000,	    0x5FFF,	    0x2080,	0x1000,
                                    0x6000,	    0x6FFF,	    0x20A0,	0x1000,
                                    0x7000,	    0x7FFF,	    0x20C0,	0x1000,
                                    0x8000,	    0x8FFF,	    0x20E0,	0x1000,
                                    0x9000,	    0x9FFF,	    0x2100,	0x1000,
                                    0xA000,	    0xAFFF,	    0x2120,	0x1000,
                                    0xB000,	    0xBFFF,	    0x2140,	0x1000,
                                    0xC000,	    0xCFFF,	    0x2160,	0x1000,
                                    0xD000,	    0xDFFF,	    0x2180,	0x1000,
                                    0xE000,	    0xEFFF,	    0x21A0,	0x1000,
                                    0xF000,	    0xFFFF,	    0x21C0,	0x1000,
                                    0x10000,	0x10FFF,	0x21E0,	0x1000,
                                    0x11000,	0x11FFF,	0x2200,	0x1000,
                                    0x12000,	0x12FFF,	0x2220,	0x1000,
                                    0x13000,	0x13FFF,	0x2240,	0x1000,
                                    0x14000,	0x14FFF,	0x2260,	0x1000,
                                    0x15000,	0x15FFF,	0x2280,	0x1000,
                                    0x16000,	0x16FFF,	0x22A0,	0x1000,
                                    0x17000,	0x17FFF,	0x22C0,	0x1000,
                                    0x18000,	0x18FFF,	0x22E0,	0x1000,
                                    0x19000,	0x19FFF,	0x2300,	0x1000,
                                    0x1A000,	0x1AFFF,	0x2320,	0x1000,
                                    0x1B000,	0x1BFFF,	0x2340,	0x1000,
                                    0x1C000,	0x1CFFF,	0x2360,	0x1000,
                                    0x1D000,	0x1DFFF,	0x2380,	0x1000,
                                    0x1E000,	0x1EFFF,	0x23A0,	0x1000,
                                    0x1F000,	0x1FFFF,	0x23C0,	0x1000,
                                    0x20000,	0x20FFF,	0x23E0,	0x1000,
                                    0x21000,	0x21FFF,	0x2400,	0x1000,
                                    0x22000,	0x22FFF,	0x2420,	0x1000,
                                    0x23000,	0x23FFF,	0x2440,	0x1000,
                                    0x24000,	0x24FFF,	0x2460,	0x1000,
                                    0x25000,	0x25FFF,	0x2480,	0x1000,
                                    0x26000,	0x26FFF,	0x24A0,	0x1000,
                                    0x27000,	0x27FFF,	0x24C0,	0x1000,
                                    0x28000,	0x28FFF,	0x24E0,	0x1000,
                                    0x29000,	0x29FFF,	0x2500,	0x1000,
                                    0x2A000,	0x2AFFF,	0x2520,	0x1000,
                                    0x2B000,	0x2BFFF,	0x2540,	0x1000,
                                    0x2C000,	0x2CFFF,	0x2560,	0x1000,
                                    0x2D000,	0x2DFFF,	0x2580,	0x1000,
                                    0x2E000,	0x2EFFF,	0x25A0,	0x1000,
                                    0x2F000,	0x2FFFF,	0x25C0,	0x1000,
                                    0x30000,	0x30FFF,	0x25E0,	0x1000,
                                    0x31000,	0x31FFF,	0x2600,	0x1000,
                                    0x32000,	0x32FFF,	0x2620,	0x1000,
                                    0x33000,	0x33FFF,	0x2640,	0x1000,
                                    0x34000,	0x34FFF,	0x2660,	0x1000,
                                    0x35000,	0x35FFF,	0x2680,	0x1000,
                                    0x36000,	0x36FFF,	0x26A0,	0x1000,
                                    0x37000,	0x37FFF,	0x26C0,	0x1000,
                                    0x38000,	0x38FFF,	0x26E0,	0x1000,
                                    0x39000,	0x39FFF,	0x2700,	0x1000,
                                    0x3A000,	0x3AFFF,	0x2720,	0x1000,
                                    0x3B000,	0x3BFFF,	0x2740,	0x1000,
                                    0x3C000,	0x3CFFF,	0x2760,	0x1000,
                                    0x3D000,	0x3DFFF,	0x2780,	0x1000,
                                    0x3E000,	0x3EFFF,	0x27A0,	0x1000,
                                    0x3F000,	0x3FFFF,	0x27C0,	0x1000,
                                    0x40000,	0x40FFF,	0x27E0,	0x1000,
                                    0x41000,	0x41FFF,	0x2800,	0x1000,
                                    0x42000,	0x42FFF,	0x2820,	0x1000,
                                    0x43000,	0x43FFF,	0x2840,	0x1000,
                                    0x44000,	0x44FFF,	0x2860,	0x1000,
                                    0x45000,	0x45FFF,	0x2880,	0x1000,
                                    0x46000,	0x46FFF,	0x28A0,	0x1000,
                                    0x47000,	0x47FFF,	0x28C0,	0x1000,
                                    0x48000,	0x48FFF,	0x28E0,	0x1000,
                                    0x49000,	0x49FFF,	0x2900,	0x1000,
                                    0x4A000,	0x4AFFF,	0x2920,	0x1000,
                                    0x4B000,	0x4BFFF,	0x2940,	0x1000,
                                    0x4C000,	0x4CFFF,	0x2960,	0x1000,
                                    0x4D000,	0x4DFFF,	0x2980,	0x1000,
                                    0x4E000,	0x4EFFF,	0x29A0,	0x1000,
                                    0x4F000,	0x4FFFF,	0x29C0,	0x1000,
                                    0x50000,	0x50FFF,	0x29E0,	0x1000,
                                    0x51000,	0x51FFF,	0x2A00,	0x1000,
                                    0x52000,	0x52FFF,	0x2A20,	0x1000,
                                    0x53000,	0x53FFF,	0x2A40,	0x1000,
                                    0x54000,	0x54FFF,	0x2A60,	0x1000,
                                    0x55000,	0x55FFF,	0x2A80,	0x1000,
                                    0x56000,	0x56FFF,	0x2AA0,	0x1000,
                                    0x57000,	0x57FFF,	0x2AC0,	0x1000,
                                    0x58000,	0x58FFF,	0x2AE0,	0x1000,
                                    0x59000,	0x59FFF,	0x2B00,	0x1000,
                                    0x5A000,	0x5AFFF,	0x2B20,	0x1000,
                                    0x5B000,	0x5BFFF,	0x2B40,	0x1000,
                                    0x5C000,	0x5CFFF,	0x2B60,	0x1000,
                                    0x5D000,	0x5DFFF,	0x2B80,	0x1000,
                                    0x5E000,	0x5EFFF,	0x2BA0,	0x1000,
                                    0x5F000,	0x5FFFF,	0x2BC0,	0x1000,
                                    0x60000,	0x60FFF,	0x2BE0,	0x1000,
                                    0x61000,	0x61FFF,	0x2C00,	0x1000,
                                    0x62000,	0x62FFF,	0x2C20,	0x1000,
                                    0x63000,	0x63FFF,	0x2C40,	0x1000,
                                    0x64000,	0x64FFF,	0x2C60,	0x1000,
                                    0x65000,	0x65FFF,	0x2C80,	0x1000,
                                    0x66000,	0x66FFF,	0x2CA0,	0x1000,
                                    0x67000,	0x67FFF,	0x2CC0,	0x1000,
                                    0x68000,	0x68FFF,	0x2CE0,	0x1000,
                                    0x69000,	0x69FFF,	0x2D00,	0x1000,
                                    0x6A000,	0x6AFFF,	0x2D20,	0x1000,
                                 };
            #endregion
            SHA256 mySHA256 = SHA256Managed.Create();

            for (int i = 0; i < hashtabledata.Length / 4; i++)
            {
                uint start = hashtabledata[0 + 4 * i];
                uint length = hashtabledata[1 + 4 * i] - hashtabledata[0 + 4 * i];
                uint offset = hashtabledata[2 + 4 * i];
                uint blocksize = hashtabledata[3 + 4 * i];

                byte[] zeroarray = new Byte[blocksize];
                Array.Copy(savefile, start, zeroarray, 0, length + 1);
                byte[] hashValue = mySHA256.ComputeHash(zeroarray);
                byte[] actualhash = new Byte[0x20];
                Array.Copy(savefile, offset, actualhash, 0, 0x20);

                if (!hashValue.SequenceEqual(actualhash))
                {
                    invalid1++;
                    RTB_S.Text += "Invalid: " + hashtabledata[2 + 4 * i].ToString("X5") + " @ " + hashtabledata[0 + 4 * i].ToString("X5") + "-" + hashtabledata[1 + 4 * i].ToString("X5") + Environment.NewLine;
                }
            }
            RTB_S.Text += "1st SAV: " + (106 - invalid1).ToString() + "/" + 106.ToString() + Environment.NewLine;

            // Check The Second Half of Hashes
            for (int i = 0; i < hashtabledata.Length; i += 4)
            {
                hashtabledata[i + 0] += 0x7F000;
                hashtabledata[i + 1] += 0x7F000;
                hashtabledata[i + 2] += 0x7F000;
            }
            // Problem with save2 saves is that 0x3000-0x4FFF doesn't use save2 data. Probably different when hashed, but different when stored.
            for (int i = 2; i < 4; i++)
            {
                hashtabledata[i * 4 + 0] -= 0x7F000;
                hashtabledata[i * 4 + 1] -= 0x7F000;
            }
            int invalid2 = 0;

            for (int i = 0; i < hashtabledata.Length / 4; i++)
            {
                uint start = hashtabledata[0 + 4 * i];
                uint length = hashtabledata[1 + 4 * i] - hashtabledata[0 + 4 * i];
                uint offset = hashtabledata[2 + 4 * i];
                uint blocksize = hashtabledata[3 + 4 * i];

                byte[] zeroarray = new Byte[blocksize];
                Array.Copy(savefile, start, zeroarray, 0, length + 1);
                byte[] hashValue = mySHA256.ComputeHash(zeroarray);
                byte[] actualhash = new Byte[0x20];
                Array.Copy(savefile, offset, actualhash, 0, 0x20);

                if (!hashValue.SequenceEqual(actualhash))
                {
                    invalid2++;
                    RTB_S.Text += "Invalid: " + hashtabledata[2 + 4 * i].ToString("X5") + " @ " + hashtabledata[0 + 4 * i].ToString("X5") + "-" + hashtabledata[1 + 4 * i].ToString("X5") + Environment.NewLine;
                }
            }
            RTB_S.Text += "2nd SAV: " + (106 - invalid2).ToString() + "/" + 106.ToString() + Environment.NewLine;

            if (invalid1 + invalid2 == (2 * 106))
                RTB_S.Text = "None of the IVFC hashes are valid." + Environment.NewLine;

            // Check the Upper Level IVFC Hashes
            {
                byte[] zeroarray = new Byte[0x200];
                Array.Copy(savefile, 0x2000, zeroarray, 0, 0x20);
                byte[] hashValue = mySHA256.ComputeHash(zeroarray);
                byte[] actualhash = new Byte[0x20];
                Array.Copy(savefile, 0x43C, actualhash, 0, 0x20);
                if (!hashValue.SequenceEqual(actualhash))
                {
                    RTB_S.Text += "Invalid: " + 0x2000.ToString("X5") + " @ " + 0x43C.ToString("X3") + Environment.NewLine;
                }
            }
            {
                byte[] zeroarray = new Byte[0x200];
                Array.Copy(savefile, 0x81000, zeroarray, 0, 0x20);
                byte[] hashValue = mySHA256.ComputeHash(zeroarray);
                byte[] actualhash = new Byte[0x20];
                Array.Copy(savefile, 0x30C, actualhash, 0, 0x20);
                if (!hashValue.SequenceEqual(actualhash))
                {
                    RTB_S.Text += "Invalid: " + 0x81000.ToString("X5") + " @ " + 0x30C.ToString("X3") + Environment.NewLine;
                }
            }
            {
                byte[] difihash1 = new Byte[0x12C];
                byte[] difihash2 = new Byte[0x12C];
                Array.Copy(savefile, 0x330, difihash1, 0, 0x12C);
                Array.Copy(savefile, 0x200, difihash2, 0, 0x12C);
                byte[] hashValue1 = mySHA256.ComputeHash(difihash1);
                byte[] hashValue2 = mySHA256.ComputeHash(difihash2);
                byte[] actualhash = new Byte[0x20];
                Array.Copy(savefile, 0x16C, actualhash, 0, 0x20);
                if (hashValue1.SequenceEqual(actualhash))
                {
                    RTB_S.Text += "Active DIFI partition is Save 1.";
                }
                else if (hashValue2.SequenceEqual(actualhash))
                {
                    RTB_S.Text += "Active DIFI partition is Save 2.";
                }
                else
                {
                    RTB_S.Text += "ERROR: NO ACTIVE DIFI HASH MATCH";
                }
            }
        }
        private void exportSAV(object sender, EventArgs e)
        {
            // Create another version of the save file.
            byte[] editedsav = new Byte[0x100000];
            Array.Copy(savefile, editedsav, savefile.Length);
            // Since we only edited one of the save files, we only have to fix half of the chk/hashes!

            // Fix Checksums
            {
                uint[] start = { 0x05400, 0x05800, 0x06400, 0x06600, 0x06800, 0x06A00, 0x06C00, 0x06E00, 0x07000, 0x07200, 0x07400, 0x09600, 0x09800, 0x09E00, 0x0A400, 0x0F400, 0x14400, 0x19400, 0x19600, 0x19E00, 0x1A400, 0x1AC00, 0x1B400, 0x1B600, 0x1B800, 0x1BE00, 0x1C000, 0x1C400, 0x1CC00, 0x1CE00, 0x1D000, 0x1D200, 0x1D400, 0x1D600, 0x1DE00, 0x1E400, 0x1E800, 0x20400, 0x20600, 0x20800, 0x20C00, 0x21000, 0x22C00, 0x23000, 0x23800, 0x23C00, 0x24600, 0x24A00, 0x25200, 0x26000, 0x26200, 0x26400, 0x27200, 0x27A00, 0x5C600, };
                uint[] length = { 0x000002C8, 0x00000B88, 0x0000002C, 0x00000038, 0x00000150, 0x00000004, 0x00000008, 0x000001C0, 0x000000BE, 0x00000024, 0x00002100, 0x00000140, 0x00000440, 0x00000574, 0x00004E28, 0x00004E28, 0x00004E28, 0x00000170, 0x0000061C, 0x00000504, 0x000006A0, 0x00000644, 0x00000104, 0x00000004, 0x00000420, 0x00000064, 0x000003F0, 0x0000070C, 0x00000180, 0x00000004, 0x0000000C, 0x00000048, 0x00000054, 0x00000644, 0x000005C8, 0x000002F8, 0x00001B40, 0x000001F4, 0x000001F0, 0x00000216, 0x00000390, 0x00001A90, 0x00000308, 0x00000618, 0x0000025C, 0x00000834, 0x00000318, 0x000007D0, 0x00000C48, 0x00000078, 0x00000200, 0x00000C84, 0x00000628, 0x00034AD0, 0x0000E058, };
                int csoff = 0x6A81A;

                if (savegame_oras)
                {
                    start = new uint[] { 0x05400, 0x05800, 0x06400, 0x06600, 0x06800, 0x06A00, 0x06C00, 0x06E00, 0x07000, 0x07200, 0x07400, 0x09600, 0x09800, 0x09E00, 0x0A400, 0x0F400, 0x14400, 0x19400, 0x19600, 0x19E00, 0x1A400, 0x1B600, 0x1BE00, 0x1C000, 0x1C200, 0x1C800, 0x1CA00, 0x1CE00, 0x1D600, 0x1D800, 0x1DA00, 0x1DC00, 0x1DE00, 0x1E000, 0x1E800, 0x1EE00, 0x1F200, 0x20E00, 0x21000, 0x21400, 0x21800, 0x22000, 0x23C00, 0x24000, 0x24800, 0x24C00, 0x25600, 0x25A00, 0x26200, 0x27000, 0x27200, 0x27400, 0x28200, 0x28A00, 0x28E00, 0x30A00, 0x38400, 0x6D000, };
                    length = new uint[] { 0x000002C8, 0x00000B90, 0x0000002C, 0x00000038, 0x00000150, 0x00000004, 0x00000008, 0x000001C0, 0x000000BE, 0x00000024, 0x00002100, 0x00000130, 0x00000440, 0x00000574, 0x00004E28, 0x00004E28, 0x00004E28, 0x00000170, 0x0000061C, 0x00000504, 0x000011CC, 0x00000644, 0x00000104, 0x00000004, 0x00000420, 0x00000064, 0x000003F0, 0x0000070C, 0x00000180, 0x00000004, 0x0000000C, 0x00000048, 0x00000054, 0x00000644, 0x000005C8, 0x000002F8, 0x00001B40, 0x000001F4, 0x000003E0, 0x00000216, 0x00000640, 0x00001A90, 0x00000400, 0x00000618, 0x0000025C, 0x00000834, 0x00000318, 0x000007D0, 0x00000C48, 0x00000078, 0x00000200, 0x00000C84, 0x00000628, 0x00000400, 0x00007AD0, 0x000078B0, 0x00034AD0, 0x0000E058, };
                    csoff = 0x7B21A;
                }

                if (savindex == 1)
                {
                    csoff += 0x7F000;
                    for (int i = 0; i < start.Length; i++)
                        start[i] += 0x7F000;
                }

                for (int i = 0; i < length.Length; i++)
                {
                    byte[] data = new Byte[length[i]];
                    Array.Copy(editedsav, start[i], data, 0, length[i]);
                    ushort checksum = PKX.ccitt16(data);
                    Array.Copy(BitConverter.GetBytes(checksum), 0, editedsav, csoff + i * 8, 2);
                }

                if (cybergadget) goto export;
            }

            // Fix Hashes
            {
                #region hash table data
                uint[] hashtabledata = {
                                    0x2020,	    0x203F,	    0x2000,	0x200,
                                    0x2040,	    0x2FFF,	    0x2020,	0x1000,
                                    0x3000,	    0x3FFF,	    0x2040,	0x1000,
                                    0x4000,	    0x4FFF,	    0x2060,	0x1000,
                                    0x5000,	    0x5FFF,	    0x2080,	0x1000,
                                    0x6000,	    0x6FFF,	    0x20A0,	0x1000,
                                    0x7000,	    0x7FFF,	    0x20C0,	0x1000,
                                    0x8000,	    0x8FFF,	    0x20E0,	0x1000,
                                    0x9000,	    0x9FFF,	    0x2100,	0x1000,
                                    0xA000,	    0xAFFF,	    0x2120,	0x1000,
                                    0xB000,	    0xBFFF,	    0x2140,	0x1000,
                                    0xC000,	    0xCFFF,	    0x2160,	0x1000,
                                    0xD000,	    0xDFFF,	    0x2180,	0x1000,
                                    0xE000,	    0xEFFF,	    0x21A0,	0x1000,
                                    0xF000,	    0xFFFF,	    0x21C0,	0x1000,
                                    0x10000,	0x10FFF,	0x21E0,	0x1000,
                                    0x11000,	0x11FFF,	0x2200,	0x1000,
                                    0x12000,	0x12FFF,	0x2220,	0x1000,
                                    0x13000,	0x13FFF,	0x2240,	0x1000,
                                    0x14000,	0x14FFF,	0x2260,	0x1000,
                                    0x15000,	0x15FFF,	0x2280,	0x1000,
                                    0x16000,	0x16FFF,	0x22A0,	0x1000,
                                    0x17000,	0x17FFF,	0x22C0,	0x1000,
                                    0x18000,	0x18FFF,	0x22E0,	0x1000,
                                    0x19000,	0x19FFF,	0x2300,	0x1000,
                                    0x1A000,	0x1AFFF,	0x2320,	0x1000,
                                    0x1B000,	0x1BFFF,	0x2340,	0x1000,
                                    0x1C000,	0x1CFFF,	0x2360,	0x1000,
                                    0x1D000,	0x1DFFF,	0x2380,	0x1000,
                                    0x1E000,	0x1EFFF,	0x23A0,	0x1000,
                                    0x1F000,	0x1FFFF,	0x23C0,	0x1000,
                                    0x20000,	0x20FFF,	0x23E0,	0x1000,
                                    0x21000,	0x21FFF,	0x2400,	0x1000,
                                    0x22000,	0x22FFF,	0x2420,	0x1000,
                                    0x23000,	0x23FFF,	0x2440,	0x1000,
                                    0x24000,	0x24FFF,	0x2460,	0x1000,
                                    0x25000,	0x25FFF,	0x2480,	0x1000,
                                    0x26000,	0x26FFF,	0x24A0,	0x1000,
                                    0x27000,	0x27FFF,	0x24C0,	0x1000,
                                    0x28000,	0x28FFF,	0x24E0,	0x1000,
                                    0x29000,	0x29FFF,	0x2500,	0x1000,
                                    0x2A000,	0x2AFFF,	0x2520,	0x1000,
                                    0x2B000,	0x2BFFF,	0x2540,	0x1000,
                                    0x2C000,	0x2CFFF,	0x2560,	0x1000,
                                    0x2D000,	0x2DFFF,	0x2580,	0x1000,
                                    0x2E000,	0x2EFFF,	0x25A0,	0x1000,
                                    0x2F000,	0x2FFFF,	0x25C0,	0x1000,
                                    0x30000,	0x30FFF,	0x25E0,	0x1000,
                                    0x31000,	0x31FFF,	0x2600,	0x1000,
                                    0x32000,	0x32FFF,	0x2620,	0x1000,
                                    0x33000,	0x33FFF,	0x2640,	0x1000,
                                    0x34000,	0x34FFF,	0x2660,	0x1000,
                                    0x35000,	0x35FFF,	0x2680,	0x1000,
                                    0x36000,	0x36FFF,	0x26A0,	0x1000,
                                    0x37000,	0x37FFF,	0x26C0,	0x1000,
                                    0x38000,	0x38FFF,	0x26E0,	0x1000,
                                    0x39000,	0x39FFF,	0x2700,	0x1000,
                                    0x3A000,	0x3AFFF,	0x2720,	0x1000,
                                    0x3B000,	0x3BFFF,	0x2740,	0x1000,
                                    0x3C000,	0x3CFFF,	0x2760,	0x1000,
                                    0x3D000,	0x3DFFF,	0x2780,	0x1000,
                                    0x3E000,	0x3EFFF,	0x27A0,	0x1000,
                                    0x3F000,	0x3FFFF,	0x27C0,	0x1000,
                                    0x40000,	0x40FFF,	0x27E0,	0x1000,
                                    0x41000,	0x41FFF,	0x2800,	0x1000,
                                    0x42000,	0x42FFF,	0x2820,	0x1000,
                                    0x43000,	0x43FFF,	0x2840,	0x1000,
                                    0x44000,	0x44FFF,	0x2860,	0x1000,
                                    0x45000,	0x45FFF,	0x2880,	0x1000,
                                    0x46000,	0x46FFF,	0x28A0,	0x1000,
                                    0x47000,	0x47FFF,	0x28C0,	0x1000,
                                    0x48000,	0x48FFF,	0x28E0,	0x1000,
                                    0x49000,	0x49FFF,	0x2900,	0x1000,
                                    0x4A000,	0x4AFFF,	0x2920,	0x1000,
                                    0x4B000,	0x4BFFF,	0x2940,	0x1000,
                                    0x4C000,	0x4CFFF,	0x2960,	0x1000,
                                    0x4D000,	0x4DFFF,	0x2980,	0x1000,
                                    0x4E000,	0x4EFFF,	0x29A0,	0x1000,
                                    0x4F000,	0x4FFFF,	0x29C0,	0x1000,
                                    0x50000,	0x50FFF,	0x29E0,	0x1000,
                                    0x51000,	0x51FFF,	0x2A00,	0x1000,
                                    0x52000,	0x52FFF,	0x2A20,	0x1000,
                                    0x53000,	0x53FFF,	0x2A40,	0x1000,
                                    0x54000,	0x54FFF,	0x2A60,	0x1000,
                                    0x55000,	0x55FFF,	0x2A80,	0x1000,
                                    0x56000,	0x56FFF,	0x2AA0,	0x1000,
                                    0x57000,	0x57FFF,	0x2AC0,	0x1000,
                                    0x58000,	0x58FFF,	0x2AE0,	0x1000,
                                    0x59000,	0x59FFF,	0x2B00,	0x1000,
                                    0x5A000,	0x5AFFF,	0x2B20,	0x1000,
                                    0x5B000,	0x5BFFF,	0x2B40,	0x1000,
                                    0x5C000,	0x5CFFF,	0x2B60,	0x1000,
                                    0x5D000,	0x5DFFF,	0x2B80,	0x1000,
                                    0x5E000,	0x5EFFF,	0x2BA0,	0x1000,
                                    0x5F000,	0x5FFFF,	0x2BC0,	0x1000,
                                    0x60000,	0x60FFF,	0x2BE0,	0x1000,
                                    0x61000,	0x61FFF,	0x2C00,	0x1000,
                                    0x62000,	0x62FFF,	0x2C20,	0x1000,
                                    0x63000,	0x63FFF,	0x2C40,	0x1000,
                                    0x64000,	0x64FFF,	0x2C60,	0x1000,
                                    0x65000,	0x65FFF,	0x2C80,	0x1000,
                                    0x66000,	0x66FFF,	0x2CA0,	0x1000,
                                    0x67000,	0x67FFF,	0x2CC0,	0x1000,
                                    0x68000,	0x68FFF,	0x2CE0,	0x1000,
                                    0x69000,	0x69FFF,	0x2D00,	0x1000,
                                    0x6A000,	0x6AFFF,	0x2D20,	0x1000,
                                 };
                #endregion
                if (savindex == 1)
                {
                    for (int i = 0; i < hashtabledata.Length/4; i++)
                    {
                        hashtabledata[i * 4 + 0] += 0x7F000;
                        hashtabledata[i * 4 + 1] += 0x7F000;
                        hashtabledata[i * 4 + 2] += 0x7F000;
                    }

                    // Problem with save2 saves is that 0x3000-0x4FFF doesn't use save2 data. Probably different when hashed, but different when stored.
                    for (int i = 2; i < 4; i++)
                    {
                        hashtabledata[i * 4 + 0] -= 0x7F000;
                        hashtabledata[i * 4 + 1] -= 0x7F000;
                    }
                }

                SHA256 mySHA256 = SHA256Managed.Create();

                // Hash for 0x3000 onwards
                for (int i = 2; i < hashtabledata.Length / 4; i++)
                {
                    uint start = hashtabledata[0 + 4 * i];
                    uint length = hashtabledata[1 + 4 * i] - hashtabledata[0 + 4 * i];
                    uint offset = hashtabledata[2 + 4 * i];
                    uint blocksize = hashtabledata[3 + 4 * i];

                    byte[] zeroarray = new Byte[blocksize];
                    Array.Copy(editedsav, start, zeroarray, 0, length + 1);
                    byte[] hashValue = mySHA256.ComputeHash(zeroarray);
                    Array.Copy(hashValue, 0, editedsav, offset, 0x20);
                }
                // Fix 2nd Hash
                {
                    uint start = hashtabledata[0 + 4 * 1];
                    uint length = hashtabledata[1 + 4 * 1] - hashtabledata[0 + 4 * 1];
                    uint offset = hashtabledata[2 + 4 * 1];
                    uint blocksize = hashtabledata[3 + 4 * 1];

                    byte[] zeroarray = new Byte[blocksize];
                    Array.Copy(editedsav, start, zeroarray, 0, length + 1);
                    byte[] hashValue = mySHA256.ComputeHash(zeroarray);

                    Array.Copy(hashValue, 0, editedsav, offset, 0x20);
                }
                // Fix 1st Hash
                {
                    uint start = hashtabledata[0 + 4 * 0];
                    uint length = hashtabledata[1 + 4 * 0] - hashtabledata[0 + 4 * 0];
                    uint offset = hashtabledata[2 + 4 * 0];
                    uint blocksize = hashtabledata[3 + 4 * 0];

                    byte[] zeroarray = new Byte[blocksize];
                    Array.Copy(editedsav, start, zeroarray, 0, length + 1);
                    byte[] hashValue = mySHA256.ComputeHash(zeroarray);

                    Array.Copy(hashValue, 0, editedsav, offset, 0x20);
                }
                // Fix IVFC Hash
                {
                    byte[] zeroarray = new Byte[0x200];
                    Array.Copy(editedsav, 0x2000 + savindex * 0x7F000, zeroarray, 0, 0x20);
                    byte[] hashValue = mySHA256.ComputeHash(zeroarray);

                    Array.Copy(hashValue, 0, editedsav, 0x43C - savindex * 0x130, 0x20);
                }
                // Fix DISA Hash
                {
                    byte[] difihash = new Byte[0x12C];
                    Array.Copy(editedsav, 0x330 - savindex * 0x130, difihash, 0, 0x12C);
                    byte[] hashValue = mySHA256.ComputeHash(difihash);

                    Array.Copy(hashValue, 0, editedsav, 0x16C, 0x20);
                }
            }
            // Write the active save index
            editedsav[0x168] = (byte)(savindex ^ 1);
          export:
            // File Integrity has been restored as well as it can. Export!

            // If CyberGadget
            if (cybergadget)
            {
                byte[] cybersav = new Byte[0x65600];
                if (savegame_oras) cybersav = new Byte[0x76000];
                Array.Copy(editedsav, 0x5400, cybersav, 0, cybersav.Length);
                // Chunk Error Checking
                byte[] FFFF = new Byte[0x200];
                byte[] section = new Byte[0x200];
                for (int i = 0; i < 0x200; i++)
                    FFFF[i] = 0xFF;
                
                for (int i = 0; i < cybersav.Length / 0x200; i++)
                {
                    Array.Copy(cybersav, i * 0x200, section, 0, 0x200);
                    if (section.SequenceEqual(FFFF))
                    {
                        string problem = String.Format("0x200 chunk @ 0x{0} is FF'd.", (i * 0x200).ToString("X5"))
                            + Environment.NewLine + "Cyber will screw up (as of August 31st)." + Environment.NewLine + Environment.NewLine;

                        // Check to see if it is in the Pokedex
                        if (i * 0x200 > 0x14E00 && i * 0x200 < 0x15700)
                        {
                            problem += "Problem lies in the Pokedex. ";
                            if (i * 0x200 == 0x15400)
                                problem += "Remove a language flag for a species ~ ex " + specieslist[548];
                        }

                        if (Util.Prompt(MessageBoxButtons.YesNo, problem, "Continue saving?") != DialogResult.Yes)
                            return;
                    }
                }   
                SaveFileDialog cySAV = new SaveFileDialog();

                // Try for file path
                string cyberpath = Util.GetTempFolder();
                if (SDFLoc != null)
                {
                    if (Directory.Exists(SDFLoc))
                    {
                        cySAV.InitialDirectory = SDFLoc;
                        cySAV.RestoreDirectory = true;
                    }
                }
                else if (Directory.Exists(Path.Combine(cyberpath, "root")))
                {
                    cySAV.InitialDirectory = Path.Combine(cyberpath, "root");
                    cySAV.RestoreDirectory = true;
                }
                else if (Directory.Exists(cyberpath))
                {
                    cySAV.InitialDirectory = cyberpath;
                    cySAV.RestoreDirectory = true;
                }

                cySAV.Filter = "Cyber SAV|*.*";
                cySAV.FileName = Regex.Split(L_Save.Text, ": ")[1];
                DialogResult sdr = cySAV.ShowDialog();
                if (sdr == DialogResult.OK)
                {
                    string path = cySAV.FileName;
                    File.WriteAllBytes(path, cybersav);
                    Util.Alert("Saved Cyber SAV to:", path);
                }
            }
            else
            {
                // Save Full Save File
                SaveFileDialog savesav = new SaveFileDialog();
                savesav.Filter = "SAV|*.bin;*.sav";
                savesav.FileName = Regex.Split(L_Save.Text, ": ")[1];
                DialogResult result = savesav.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string path = savesav.FileName;

                    if (File.Exists(path))
                    {
                        // File already exists, save a .bak
                        byte[] backupfile = File.ReadAllBytes(path);
                        File.WriteAllBytes(path + ".bak", backupfile);
                    }
                    File.WriteAllBytes(path, editedsav);
                    Util.Alert("Saved 1MB SAV to:", path);
                }
            }
        }
        // Box/SAV Functions // 
        private void B_BoxRight_Click(object sender, EventArgs e)
        {
            if (C_BoxSelect.SelectedIndex < 30)
                C_BoxSelect.SelectedIndex++;
            else C_BoxSelect.SelectedIndex = 0;
        }
        private void B_BoxLeft_Click(object sender, EventArgs e)
        {
            if (C_BoxSelect.SelectedIndex > 0)
                C_BoxSelect.SelectedIndex--;
            else C_BoxSelect.SelectedIndex = 30;
        }
        private void rcmView_Click(object sender, EventArgs e)
        {
            int slot = getSlot(sender);
            int offset = getPKXOffset(slot);

            PictureBox[] pba = {
                                    bpkx1, bpkx2, bpkx3, bpkx4, bpkx5, bpkx6,
                                    bpkx7, bpkx8, bpkx9, bpkx10,bpkx11,bpkx12,
                                    bpkx13,bpkx14,bpkx15,bpkx16,bpkx17,bpkx18,
                                    bpkx19,bpkx20,bpkx21,bpkx22,bpkx23,bpkx24,
                                    bpkx25,bpkx26,bpkx27,bpkx28,bpkx29,bpkx30,

                                    ppkx1, ppkx2, ppkx3, ppkx4, ppkx5, ppkx6,
                                    bbpkx1,bbpkx2,bbpkx3,bbpkx4,bbpkx5,bbpkx6,

                                    dcpkx1, dcpkx2, gtspkx, fusedpkx,subepkx1,subepkx2,subepkx3,
                                };

            PictureBox picturebox = pba[slot];
            if (picturebox.Image == null)
            { System.Media.SystemSounds.Exclamation.Play(); return; }

            // Load the PKX file
            if (BitConverter.ToUInt64(savefile, offset + 8) != 0)
            {
                byte[] ekxdata = new Byte[0xE8];
                Array.Copy(savefile, offset, ekxdata, 0, 0xE8);
                byte[] pkxdata = PKX.decryptArray(ekxdata);
                int species = BitConverter.ToInt16(pkxdata, 0x08); // Get Species
                if (species == 0)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    return;
                }
                try
                {
                    Array.Copy(pkxdata, buff, 0xE8);
                    populateFields(buff);
                }
                catch // If it fails, try XORing encrypted zeroes
                {
                    try
                    {
                        byte[] blank = PKX.encryptArray(new Byte[0xE8]);

                        for (int i = 0; i < 0xE8; i++)
                            blank[i] = (byte)(buff[i] ^ blank[i]);

                        populateFields(blank);
                    }
                    catch   // Still fails, just let the original errors occur.
                    { populateFields(buff); }
                }
                // Visual to display what slot is currently loaded.
                getSlotColor(slot, Properties.Resources.slotView);
            }
            else
                System.Media.SystemSounds.Exclamation.Play();
        }
        private void rcmSet_Click(object sender, EventArgs e)
        {
            if (!verifiedpkx()) { return; }
            int slot = getSlot(sender);
            if (slot == 30 && (CB_Species.SelectedIndex == 0 || CHK_IsEgg.Checked)) { Util.Alert("Can't have empty/egg first slot."); return; }
            int offset = getPKXOffset(slot);

            byte[] pkxdata = preparepkx(buff);
            byte[] ekxdata = PKX.encryptArray(pkxdata);

            if (!savegame_oras)
            {
                // User Protection
                int move1 = BitConverter.ToInt16(pkxdata,0x5A);
                int move2 = BitConverter.ToInt16(pkxdata,0x5C);
                int move3 = BitConverter.ToInt16(pkxdata,0x5E);
                int move4 = BitConverter.ToInt16(pkxdata,0x60);
                int ability = pkxdata[0x14];
                int item = BitConverter.ToInt16(pkxdata,0x0A);
                string err = "";

                if (move1 > 617 || move2 > 617 || move3 > 617 || move4 > 617)
                    err = "Move does not exist in X/Y.";
                else if (ability > 188)
                    err = "Ability does not exist in X/Y.";
                else if (item > 717)
                    err = "Item does not exist in X/Y.";
                else goto next;

                if (Util.Prompt(MessageBoxButtons.YesNo, err, "Continue?") != DialogResult.Yes)
                    return;
            }
          next:
            if (slot >= 30 && slot < 36) // Party
                Array.Copy(ekxdata, 0, savefile, offset, 0x104);
            else if (slot < 30 || (slot >= 36 && slot < 42 && DEV_Ability.Enabled))
                Array.Copy(ekxdata, 0, savefile, offset, 0xE8);
            else return;

            setParty();
            setPokedex(pkxdata);
            setPKXBoxes();
            savedited = true;

            getSlotColor(slot, Properties.Resources.slotSet);
        }
        private void rcmDelete_Click(object sender, EventArgs e)
        {
            byte partycount = setParty();
            int slot = getSlot(sender);
            if (slot == 30 && partycount == 1 && !DEV_Ability.Enabled) { Util.Alert("Can't delete first slot."); return; }
            int offset = getPKXOffset(slot);

            byte[] pkxdata = new Byte[0x104];
            byte[] ekxdata = PKX.encryptArray(pkxdata);

            if (slot >= 30 && slot < 36) // Party
                Array.Copy(ekxdata, 0, savefile, offset, 0x104);
            else if (slot < 30 || (slot >= 36 && slot < 42 && DEV_Ability.Enabled))
                Array.Copy(ekxdata, 0, savefile, offset, 0xE8);
            else return;

            setParty();
            setPKXBoxes();
            savedited = true;

            getSlotColor(slot, Properties.Resources.slotDel);
        }
        private void cloneBox(object sender, EventArgs e)
        {
            if (!verifiedpkx()) { return; } // don't copy garbage to the box
            if (getSlot(sender) > 30) return; // only perform action if cloning to boxes

            int box = C_BoxSelect.SelectedIndex + 1; // get box we're cloning to
            {
                DialogResult sdr = Util.Prompt(MessageBoxButtons.YesNo, String.Format("Clone Pokemon from Editing Tabs to all slots in Box {0}?", box));
                if (sdr != DialogResult.Yes) // give the option to abort
                    return;
            }

            byte[] pkxdata = preparepkx(buff);
            byte[] ekxdata = PKX.encryptArray(pkxdata);
            for (int i = 0; i < 30; i++) // write encrypted array to all box slots
                Array.Copy(ekxdata, 0, savefile, getPKXOffset(i), 0xE8);

            setPokedex(pkxdata); // set pokedex data if necessary
            setPKXBoxes();  // refresh box view
            savedited = true;
        }
        private void setPokedex(byte[] pkxdata)
        {
            if (savindex > 1) return;
            int species = BitConverter.ToUInt16(pkxdata, 0x8);  // Species
            int lang = pkxdata[0xE3] - 1; if (lang > 5) lang--; // 0-6 language vals
            int origin = pkxdata[0xDF];                         // Native / Non Native
            int gender = (pkxdata[0x1D] & 2) >> 1;              // Gender
            uint pid = BitConverter.ToUInt32(pkxdata, 0x18);
            ushort TID = BitConverter.ToUInt16(pkxdata, 0xC);
            ushort SID = BitConverter.ToUInt16(pkxdata, 0xE);
            int shiny = Convert.ToInt16(Convert.ToBoolean((PKX.getPSV(pid) ^ PKX.getTSV(TID,SID)) < 16));
            int dexoff = savindex * 0x7F000 + SaveGame.PokeDex; // Same offset for XY-ORAS
            int langoff = 0x3C8; if (savegame_oras) langoff = 0x400; // Not the same offset for language bools
            int shiftoff = (shiny * 0x60 * 2) + (gender * 0x60) + 0x60;

            // Set the [Species/Gender/Shiny] Owned Flag
            savefile[dexoff + shiftoff + (species - 1) / 8 + 0x8] |= (byte)(1 << ((species - 1) % 8));

            // Owned quality flag
            if (origin < 0x18 && species < 650 && !savegame_oras) // Pre 650 for X/Y, and not for ORAS; Set the Foreign Owned Flag
                savefile[0x1AA4C + 0x7F000 * savindex + (species - 1) / 8] |= (byte)(1 << ((species - 1) % 8));
            else if (origin >= 0x18 || savegame_oras) // Set Native Owned Flag (should always happen)
                savefile[dexoff + (species - 1) / 8 + 0x8] |= (byte)(1 << ((species - 1) % 8));

            // Set the Language
            if (lang < 0) lang = 1;
                savefile[dexoff + langoff + ((species - 1) * 7 + lang) / 8] |= (byte)(1 << ((((species - 1) * 7) + lang) % 8));
        }
        private byte setParty()
        {
            byte partymembers = 0; // start off with a ctr of 0
            int offset = SaveGame.Party + 0x7F000 * savindex;
            for (int i = 0; i < 6; i++)
            {
                // Gather all the species
                byte[] data = new Byte[0x104];
                Array.Copy(savefile, offset + i * 0x104, data, 0, 0x104);
                byte[] decdata = PKX.decryptArray(data);
                int species = BitConverter.ToInt16(decdata,8);
                if ((species != 0) && (species < 722))
                {
                    Array.Copy(data, 0, savefile, offset + (partymembers) * 0x104, 0x104);
                    partymembers++; // Copy in our party member
                }
            }

            // Write in the current party count
            savefile[offset + 6 * 0x104 + savindex * 0x7F000] = partymembers;
            // Zero out the party slots that are empty.
            for (int i = 0; i < 6; i++)
                if (i >= partymembers)
                    Array.Copy(PKX.encryptArray(new Byte[0x104]), 0, savefile, offset + (i * 0x104), 0x104);

            // Repeat for Battle Box.
            byte battlemem = 0;
            int offset2 = SaveGame.BattleBox + 0x7F000 * savindex;
            for (int i = 0; i < 6; i++)
            {
                // Gather all the species
                byte[] data = new Byte[0x104];
                Array.Copy(savefile, offset2 + i * 0xE8, data, 0, 0xE8);
                byte[] decdata = PKX.decryptArray(data);
                int species = BitConverter.ToInt16(decdata, 8);
                if ((species != 0) && (species < 722))
                {
                    Array.Copy(data, 0, savefile, offset2 + (battlemem) * 0xE8, 0xE8);
                    battlemem++; // Copy in our party member
                }
            }

            // Zero out the party slots that are empty.
            for (int i = 0; i < 6; i++)
                if (i >= battlemem)
                    Array.Copy(PKX.encryptArray(new Byte[0x104]), 0, savefile, offset2 + (i * 0xE8), 0xE8);

            if (battlemem == 0)
                savefile[offset2 + 6 * 0xE8 + savindex * 0x7F000] = 0;

            return partymembers;
        }
        private void slotModifier_Click(object sender, EventArgs e)
        {
            if (ModifierKeys == (Keys.Control | Keys.Alt))
                cloneBox(sender, e);
            else if (ModifierKeys == Keys.Control)
                rcmView_Click(sender, e);
            else if (ModifierKeys == Keys.Shift)
                rcmSet_Click(sender, e);
            else if (ModifierKeys == Keys.Alt)
                rcmDelete_Click(sender, e);
        }
        // Subfunctions // 
        private int getPKXOffset(int slot)
        {
            int offset = SaveGame.Box + C_BoxSelect.SelectedIndex * (0xE8 * 30) + slot * 0xE8;

            if (slot > 29)          // Not a party
            {
                if (slot < 36)      // Party Slot
                    offset = SaveGame.Party + (slot - 30) * 0x104;
                else if (slot < 42) // Battle Box Slot
                    offset = SaveGame.BattleBox + (slot - 36) * 0xE8;
                else if (slot < 44) // Daycare
                    offset = SaveGame.Daycare + 8 + (slot - 42) * 0xF0;
                else if (slot < 45) // GTS
                    offset = SaveGame.GTS;
                else if (slot < 46) // Fused
                    offset = SaveGame.Fused;
                else                // SUBE
                    offset = SaveGame.SUBE + (slot - 46) * 0xEC;
            }
            offset += 0x7F000 * savindex;
            return offset;
        }
        private int getSlot(object sender)
        {
            Control sourceControl = null;
            // Try to cast the sender to a ToolStripItem
            try { ToolStripItem menuItem = sender as ToolStripItem; ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip; sourceControl = owner.SourceControl; }
            catch 
            { // try to cast as picturebox 
                try { PictureBox pbItem = sender as PictureBox; sourceControl = pbItem as Control; }
                catch 
                { Util.Error("Invalid slot!", "getSlot could not cast the control element."); return 0; }
            }

            string[] pba = {
                                "bpkx1", "bpkx2", "bpkx3", "bpkx4", "bpkx5", "bpkx6",
                                "bpkx7", "bpkx8", "bpkx9", "bpkx10","bpkx11","bpkx12",
                                "bpkx13","bpkx14","bpkx15","bpkx16","bpkx17","bpkx18",
                                "bpkx19","bpkx20","bpkx21","bpkx22","bpkx23","bpkx24",
                                "bpkx25","bpkx26","bpkx27","bpkx28","bpkx29","bpkx30",

                                "ppkx1", "ppkx2", "ppkx3", "ppkx4", "ppkx5", "ppkx6",
                                "bbpkx1","bbpkx2","bbpkx3","bbpkx4","bbpkx5","bbpkx6",

                                "dcpkx1", "dcpkx2", "gtspkx", "fusedpkx","subepkx1","subepkx2","subepkx3",
                            };
            int slot = Array.IndexOf(pba, sourceControl.Name);
            return slot;
        }
        public void setPKXBoxes()
        {
            int boxoffset = SaveGame.Box + 0x7F000 * savindex + C_BoxSelect.SelectedIndex * (0xE8 * 30);

            int boxbgofst = (0x7F000 * savindex) + 0x9C1E + C_BoxSelect.SelectedIndex;
            int boxbgval = 1 + savefile[boxbgofst];
            string imagename = "box_wp" + boxbgval.ToString("00"); if (savegame_oras && boxbgval > 16) imagename += "o";
            PAN_Box.BackgroundImage = (Image)Properties.Resources.ResourceManager.GetObject(imagename);
            
            PictureBox[] pba = {
                                    bpkx1, bpkx2, bpkx3, bpkx4, bpkx5, bpkx6,
                                    bpkx7, bpkx8, bpkx9, bpkx10,bpkx11,bpkx12,
                                    bpkx13,bpkx14,bpkx15,bpkx16,bpkx17,bpkx18,
                                    bpkx19,bpkx20,bpkx21,bpkx22,bpkx23,bpkx24,
                                    bpkx25,bpkx26,bpkx27,bpkx28,bpkx29,bpkx30,

                                    ppkx1, ppkx2, ppkx3, ppkx4, ppkx5, ppkx6,
                                    bbpkx1,bbpkx2,bbpkx3,bbpkx4,bbpkx5,bbpkx6,

                                    dcpkx1, dcpkx2, gtspkx, fusedpkx,subepkx1,subepkx2,subepkx3,
                                };
            for (int i = 0; i < 30; i++)
            {
                int offset = boxoffset + 0xE8 * i;
                getSlotFiller(offset, pba[i]);
            }

            // Reload Party
            for (int i = 0; i < 6; i++)
            {
                int offset = SaveGame.Party + (0x7F000 * savindex) + 0x104 * i;
                getSlotFiller(offset, pba[i + 30]);
            }

            // Reload Battle Box
            for (int i = 0; i < 6; i++)
            {
                int offset = SaveGame.BattleBox + (0x7F000 * savindex) + 0xE8 * i;
                getSlotFiller(offset, pba[i + 36]);
            }

            // Reload Daycare
            Label[] dclabela = { L_DC1, L_DC2, };
            TextBox[] dctexta = { TB_Daycare1XP, TB_Daycare2XP };

            for (int i = 0; i < 2; i++)
            {
                int offset = SaveGame.Daycare + (0x7F000 * savindex) + 0xE8 * i + 8 * (i + 1);
                getSlotFiller(offset, pba[i + 42]);
                dctexta[i].Text = BitConverter.ToUInt32(savefile, SaveGame.Daycare + (0x7F000 * savindex) + 0xF0 * i + 4).ToString();
                if (Convert.ToBoolean(savefile[SaveGame.Daycare + (0x7F000 * savindex) + 0xF0 * i]))   // If Occupied
                {
                    pba[i + 42].Image = Util.ChangeOpacity(pba[i + 42].Image, 1);
                    dclabela[i].Text = (i + 1) + ": Occupied";
                }
                else
                {
                    pba[i + 42].Image = Util.ChangeOpacity(pba[i + 42].Image, 0.6);
                    dclabela[i].Text = (i + 1) + ": Not Occupied";
                }
            }
            DayCare_HasEgg.Checked = Convert.ToBoolean(savefile[SaveGame.Daycare + (0x7F000 * savindex) + 0x1E0]);
            TB_RNGSeed.Text = BitConverter.ToUInt64(savefile, SaveGame.Daycare + (0x7F000 * savindex) + 0x1E8).ToString("X16");

            // GTS
            getSlotFiller(SaveGame.GTS + (0x7F000 * savindex), pba[44]);

            // Fused
            getSlotFiller(SaveGame.Fused + (0x7F000 * savindex), pba[45]);

            // SUBE
            for (int i = 0; i < 3; i++)
            {
                int offset = 0x22C90 + i * 0xEC + (0x7F000 * savindex);
                if (BitConverter.ToUInt64(savefile, offset) != 0)
                    getSlotFiller(offset, pba[46 + i]);
                else pba[46 + i].Image = null;
            }

            // Recoloring of a storage box slot (to not show for other storage boxes)
            if (colorizedslot < 32)
            {
                if (colorizedbox == C_BoxSelect.SelectedIndex)
                    pba[colorizedslot].BackgroundImage = colorizedcolor;
                else
                    pba[colorizedslot].BackgroundImage = null;
            }
        }
        public void setBoxNames()
        {
            int selectedbox = C_BoxSelect.SelectedIndex;    // precache selected box
            // Build ComboBox Dropdown Items
            try
            {
                C_BoxSelect.Items.Clear();
                for (int i = 0; i < 31; i++)
                {
                    string boxname = Encoding.Unicode.GetString(savefile, SaveGame.PCLayout + (0x7F000 * savindex) + 0x22 * i, 0x22);
                    C_BoxSelect.Items.Add(boxname);
                }
            }
            catch
            {
                for (int i = 1; i < 32; i++)
                    C_BoxSelect.Items.Add("Box " + i);
            }
            C_BoxSelect.SelectedIndex = selectedbox;    // restore selected box
        }
        private void setSAVLabel()
        {
            L_SAVINDEX.Text = (savindex + 1).ToString();
            RTB_S.AppendText("Loaded Save File " + (savindex + 1).ToString() + Environment.NewLine);
        }
        private void getSAVOffsets()
        {
            // Get the save file offsets for the input game
            bool enableInterface = false;
            if (BitConverter.ToUInt32(savefile, 0x6A810 + 0x7F000 * savindex) == 0x42454546)
            {
                enableInterface = true;
                SaveGame = new SaveGames.SaveStruct("XY");
            }
            else
            {
                Util.Error("Unrecognized Save File loaded.");
                SaveGame = new SaveGames.SaveStruct("Error");
            }

            // Enable Buttons
            GB_SAVtools.Enabled = B_JPEG.Enabled = B_BoxIO.Enabled = B_VerifyCHK.Enabled = B_VerifySHA.Enabled = B_SwitchSAV.Enabled
                = enableInterface;
        }
        private void getSlotFiller(int offset, PictureBox pb)
        {
            byte[] slotdata = new Byte[0xE8];
            Array.Copy(savefile, offset, slotdata, 0, 0xE8);    // Fill Our EKX Slot
            byte[] dslotdata = PKX.decryptArray(slotdata);

            ushort chk = 0;
            for (int i = 8; i < 232; i += 2) // Loop through the entire PKX
                chk += BitConverter.ToUInt16(dslotdata, i);

            if (chk != BitConverter.ToUInt16(dslotdata, 6) && (!savLoaded && !slotdata.SequenceEqual(new Byte[0xE8]))) // Bad Egg
            {
                pb.Image = null;
                pb.BackColor = Color.Red;
                return;
            }
            else pb.BackColor = Color.Transparent;
            int species = BitConverter.ToInt16(dslotdata, 0x08); // Get Species
            uint isegg = (BitConverter.ToUInt32(dslotdata, 0x74) >> 30) & 1;

            int altforms = (dslotdata[0x1D] >> 3);
            int gender = (dslotdata[0x1D] >> 1) & 0x3;

            string file;

            if (species == 0)
            { pb.Image = (Image)Properties.Resources.ResourceManager.GetObject("_0"); return; }

            else
            {
                file = "_" + species.ToString();
                if (altforms > 0) // Alt Form Handling
                    file = file + "_" + altforms.ToString();
                else if ((gender == 1) && (species == 521 || species == 668))   // Unfezant & Pyroar
                    file = file = "_" + species.ToString() + "f";
            }

            // Redrawing logic
            uint PID = BitConverter.ToUInt32(dslotdata, 0x18);
            ushort PSV = PKX.getPSV(PID);
            ushort TSV = PKX.getTSV(BitConverter.ToUInt16(dslotdata, 0x0C),BitConverter.ToUInt16(dslotdata, 0x0E));
            ushort XOR = (ushort)(TSV ^ PSV);

            Image baseImage = (Image)Properties.Resources.ResourceManager.GetObject(file);
            if (baseImage == null)
            {
                if (species < 722)
                {
                    baseImage = PKHeX.Util.LayerImage(
                        (Image)Properties.Resources.ResourceManager.GetObject("_" + species.ToString()),
                        (Image)Properties.Resources.unknown, 
                        0, 0, .5);
                }
                else 
                    baseImage = (Image)Properties.Resources.unknown;
            }
            if (isegg == 1)
            {
                file = "_0"; // Start with a partially transparent species 
                baseImage = PKHeX.Util.LayerImage((Image)Properties.Resources.ResourceManager.GetObject(file), baseImage, 0, 0, 0.33);
                file = "egg"; // Add the egg layer over-top.
                baseImage = PKHeX.Util.LayerImage(baseImage, (Image)Properties.Resources.ResourceManager.GetObject(file), 0, 0, 1);
            }
            if (XOR == 0)
            {   // Is Shiny
                // Redraw our image
                baseImage = PKHeX.Util.LayerImage(baseImage, Properties.Resources.rare_icon, 0, 0, 0.7);
            }
            if (BitConverter.ToUInt16(dslotdata, 0xA) > 0)
            {
                // Has Item
                int item = BitConverter.ToUInt16(dslotdata, 0xA);
                Image itemimg = (Image)Properties.Resources.ResourceManager.GetObject("item_"+item.ToString());
                if (itemimg == null) itemimg = Properties.Resources.helditem;
                // Redraw
                baseImage = PKHeX.Util.LayerImage(baseImage, itemimg, 22 + (15-itemimg.Width)/2, 15 + (15-itemimg.Height), 1);
            }

            pb.Image = baseImage;
        }
        private void getSlotColor(int slot, Image color)
        {
            PictureBox[] pba = {
                                    bpkx1, bpkx2, bpkx3, bpkx4, bpkx5, bpkx6,
                                    bpkx7, bpkx8, bpkx9, bpkx10,bpkx11,bpkx12,
                                    bpkx13,bpkx14,bpkx15,bpkx16,bpkx17,bpkx18,
                                    bpkx19,bpkx20,bpkx21,bpkx22,bpkx23,bpkx24,
                                    bpkx25,bpkx26,bpkx27,bpkx28,bpkx29,bpkx30,

                                    ppkx1, ppkx2, ppkx3, ppkx4, ppkx5, ppkx6,
                                    bbpkx1,bbpkx2,bbpkx3,bbpkx4,bbpkx5,bbpkx6,

                                    dcpkx1, dcpkx2, gtspkx, fusedpkx,subepkx1,subepkx2,subepkx3,
                                };

            for (int i = 0; i < pba.Length; i++)
                pba[i].BackgroundImage = null;

            if (slot < 32)
                colorizedbox = C_BoxSelect.SelectedIndex;

            pba[slot].BackgroundImage = color;
            colorizedcolor = color;
            colorizedslot = slot;
        }
        private void getBox(object sender, EventArgs e)
        {
            setPKXBoxes();
        }
        private void getTSV(object sender, EventArgs e)
        {
            uint TID = Util.ToUInt32(TB_TID.Text);
            uint SID = Util.ToUInt32(TB_SID.Text);
            uint tsv = PKX.getTSV((ushort)TID, (ushort)SID);
            Tip1.SetToolTip(this.TB_TID, "TSV: " + tsv.ToString("0000"));
            Tip2.SetToolTip(this.TB_SID, "TSV: " + tsv.ToString("0000"));

            uint PID = Util.getHEXval(TB_PID);
            uint psv = PKX.getPSV(PID);
            Tip3.SetToolTip(this.TB_PID, "PSV: " + psv.ToString("0000"));
        }
        private void Menu_DumpLoadBoxes_Click(object sender, EventArgs e)
        {
            DialogResult dr = Util.Prompt(MessageBoxButtons.YesNoCancel, "Press Yes to Import All from Folder." + Environment.NewLine + "Press No to Dump All to Folder.", "Press Cancel to Abort.");
            if (dr != DialogResult.Cancel)
            {
                string exepath = System.Windows.Forms.Application.StartupPath;
                string path = "";
                {
                    int offset = SaveGame.Box;
                    int size = 232;
                    if (dr == DialogResult.Yes) // Import
                    {
                        if (Directory.Exists(Path.Combine(exepath, "db")))
                        {
                            DialogResult ld = Util.Prompt(MessageBoxButtons.YesNo, "Load from PKHeX's database?");
                            if (ld == DialogResult.Yes)
                                path = Path.Combine(exepath, "db");
                            else if (ld == DialogResult.No)
                            {
                                // open folder dialog
                                FolderBrowserDialog fbd = new FolderBrowserDialog();
                                if (fbd.ShowDialog() == DialogResult.OK)
                                    path = fbd.SelectedPath;
                            }
                            else return;
                        }
                        else
                        {
                            // open folder dialog
                            FolderBrowserDialog fbd = new FolderBrowserDialog();
                            if (fbd.ShowDialog() == DialogResult.OK)
                                path = fbd.SelectedPath;
                        }
                        loadBoxesFromDB(path);
                    }
                    else if (dr == DialogResult.No)
                    {
                        // Dump all of box content to files.
                        {
                            DialogResult ld = Util.Prompt(MessageBoxButtons.YesNo, "Save to PKHeX's database?");
                            if (ld == DialogResult.Yes)
                            {
                                path = Path.Combine(exepath, "db");
                                if (!Directory.Exists(path))
                                    Directory.CreateDirectory(path);
                            }
                            else if (ld == DialogResult.No)
                            {
                                // open folder dialog
                                FolderBrowserDialog fbd = new FolderBrowserDialog();
                                if (fbd.ShowDialog() == DialogResult.OK)
                                    path = fbd.SelectedPath;
                            }
                            else return;
                        }
                        for (int i = 0; i < 31 * 30 * size; i += size)
                        {
                            byte[] ekxdata = new Byte[size];
                            Array.Copy(savefile, offset + i, ekxdata, 0, size);
                            byte[] pkxdata = PKX.decryptArray(ekxdata);


                            int species = BitConverter.ToInt16(pkxdata, 0x08);
                            if (species == 0) continue;
                            uint chk = BitConverter.ToUInt16(pkxdata, 0x06);
                            uint EC = BitConverter.ToUInt32(pkxdata, 0);
                            uint IV32 = BitConverter.ToUInt32(pkxdata, 0x74);

                            string nick = "";
                            if (Convert.ToBoolean((IV32 >> 31) & 1))
                                nick = Util.TrimFromZero(Encoding.Unicode.GetString(pkxdata, 0x40, 24)) + " (" + specieslist[species] + ")";
                            else
                                nick = specieslist[species];
                            if (Convert.ToBoolean((IV32 >> 30) & 1))
                                nick += " (" + eggname + ")";

                            string isshiny = "";
                            int gamevers = pkxdata[0xDF];

                            uint PID = BitConverter.ToUInt32(pkxdata, 0x18);
                            uint UID = (PID >> 16);
                            uint LID = (PID & 0xFFFF);
                            uint PSV = UID ^ LID;
                            uint TSV = Util.ToUInt32(TB_TID.Text) ^ Util.ToUInt32(TB_SID.Text);
                            uint XOR = TSV ^ PSV;
                            if (XOR < 16) // Is Shiny
                                isshiny = " ★";

                            string savedname =
                                species.ToString("000") + isshiny + " - "
                                + nick + " - "
                                + chk.ToString("X4") + EC.ToString("X8")
                                + ".pk6";
                            Array.Resize(ref pkxdata, 232);
                            if (!File.Exists(Path.Combine(path, savedname)))
                                File.WriteAllBytes(Path.Combine(path, Util.CleanFileName(savedname)), pkxdata);
                        }
                    }
                }
            }
        }
        private void loadBoxesFromDB(string path)
        {
            if (path == "") return;
            int offset = SaveGame.Box;
            int ctr = C_BoxSelect.SelectedIndex * 30;
            int pastctr = 0;

            // Clear out the box data array.
            // Array.Clear(savefile, offset, size * 30 * 31);
            if (Util.Prompt(MessageBoxButtons.YesNo, "Clear subsequent boxes when importing data?", "If you only want to overwrite for new data, press no.") == DialogResult.Yes)
            {
                byte[] ezeros = PKX.encryptArray(new Byte[232]);
                for (int i = ctr; i < 30 * 31; i++)
                    Array.Copy(ezeros, 0, savefile, offset + i * 232, 232);
            }
            string[] filepaths = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
            var Converter = new pk2pk();

            for (int i = 0; i < filepaths.Length; i++)
            {
                long len = new FileInfo(filepaths[i]).Length;
                if (len > 260) continue;
                else if (
                       len != 232 && len != 260 // 6th Gen
                    && len != 136 && len != 220 && len != 236 // 5th Gen
                    && len != 100 && len != 80) // 4th Gen
                    continue;
                string name = filepaths[i];
                byte[] data = new Byte[232];
                string ext = Path.GetExtension(filepaths[i]);
                if (ext == ".pkm" || ext == ".3gpkm" || ext == ".pk3" || ext == ".pk4" || ext == ".pk5")
                {
                    // Verify PKM (decrypted)
                    byte[] input = File.ReadAllBytes(filepaths[i]);
                    if (!PKX.verifychk(input))
                        continue;
                    else
                    {
                        try // to convert g5pkm
                        { data = PKX.encryptArray(Converter.ConvertPKM(input, savefile, savindex)); pastctr++; }
                        catch 
                        { continue; }
                    }
                }
                else if (ext == ".pkx" || ext == ".pk6")
                {
                    byte[] input = File.ReadAllBytes(filepaths[i]);
                    if ((BitConverter.ToUInt16(input, 0xC8) == 0) && (BitConverter.ToUInt16(input,0x58) == 0))
                    {
                        if (BitConverter.ToUInt16(input, 0x8) == 0) // if species = 0
                            continue;
                        Array.Resize(ref input, 232);

                        ushort chk = 0;
                        for (int z = 8; z < 232; z += 2) // Loop through the entire PKX
                            chk += BitConverter.ToUInt16(input, z);
                        if (chk != BitConverter.ToUInt16(input, 0x6)) continue;
                        data = PKX.encryptArray(input);
                    }
                }
                else if (ext == ".ekx" || ext == ".ek6")
                {
                    byte[] input = File.ReadAllBytes(filepaths[i]);
                    Array.Resize(ref input, 232);
                    Array.Copy(input, data, 232);
                    // check if it is good data
                    byte[] decrypteddata = PKX.decryptArray(input);

                    if (!(BitConverter.ToUInt16(decrypteddata, 0xC8) == 0) && !(BitConverter.ToUInt16(decrypteddata, 0x58) == 0))
                        continue; // don't allow improperly encrypted files. they must be encrypted properly.
                    else if (BitConverter.ToUInt16(decrypteddata, 0x8) == 0) // if species = 0
                        continue;

                    ushort chk = 0;
                    for (int z = 8; z < 232; z += 2) // Loop through the entire PKX
                        chk += BitConverter.ToUInt16(decrypteddata, z);
                    if (chk != BitConverter.ToUInt16(decrypteddata, 0x6)) continue;
                }
                else continue;
                Array.Copy(data, 0, savefile, offset + ctr * 232, 232);
                setPokedex(PKX.decryptArray(data)); // Set the Pokedex data
                ctr++;
                if (ctr == 30 * 31) break; // break out if we have written all 31 boxes
            }
            if (ctr > 0) // if we've written at least one pk6 in, go ahead and make sure the window is stretched.
            {
                if (Width < Height) // expand if boxes aren't visible
                {
                    this.Width = largeWidth;
                    tabBoxMulti.Enabled = true;
                    tabBoxMulti.SelectedIndex = 0;
                }
                setPKXBoxes();
                string result = String.Format("Loaded {0} files to boxes.", ctr);
                if (pastctr > 0)
                    Util.Alert(result, String.Format("Conversion successful for {0} past generation files.", pastctr));
                else
                    Util.Alert(result);
            }
        }
        // Subfunction Save Buttons // 
        private void B_OpenWondercards_Click(object sender, EventArgs e)
        {
            // Open Wondercard Menu
            new SAV_Wondercard(this).ShowDialog();
        }
        private void B_OpenBoxLayout_Click(object sender, EventArgs e)
        {
            // Open Box Layout Menu
            new SAV_BoxLayout(this).ShowDialog();
            setBoxNames(); // fix box names
            setPKXBoxes(); // refresh box background
        }
        private void B_OpenTrainerInfo_Click(object sender, EventArgs e)
        {
            new SAV_Trainer(this).ShowDialog();
        }
        private void B_OpenPokepuffs_Click(object sender, EventArgs e)
        {
            new SAV_Pokepuff(this).ShowDialog();
        }
        private void B_OpenItemPouch_Click(object sender, EventArgs e)
        {
            new SAV_Inventory(this).ShowDialog();
        }
        private void B_OpenBerryField_Click(object sender, EventArgs e)
        {
            if (savegame_oras)
            {
                DialogResult dr = Util.Prompt(MessageBoxButtons.YesNo, "No editing support for ORAS :(", "Repopulate all with random berries?");
                if (dr == DialogResult.Yes)
                {
                    // Randomize the trees.
                    int offset = 0x1C400 + 0x5400 + savindex * 0x7F000;
                    byte[] ready = new byte[] { 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0x80, 0x40, 0x01, 0x00, 0x00, 0x00, };
                    int[] berrylist = new int[] 
                    {
                        0,149,150,151,152,153,154,155,156,157,158,159,160,161,162,
                        163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,
                        178,179,180,181,182,183,184,185,186,187,188,189,190,191,192,
                        193,194,195,196,197,198,199,200,201,202,203,204,205,206,207,
                        208,209,210,211,212,686,687,688,
                    };
                    for (int i = 0; i < 90; i++)
                    {
                        Array.Copy(ready, 0, savefile, offset + 0x10 * i, 0x10); // prep the berry template tree (which we replace offset 0x6 for the Tree Item)
                        int randberry = (int)(Util.rnd32() % berrylist.Length); // generate a random berry that will go into the tree
                        int index = berrylist[randberry]; // get berry item ID from list
                        Array.Copy(BitConverter.GetBytes(index), 0, savefile, offset + 0x10 * i + 6, 2); // put berry into tree.
                    }
                }
            }
            else 
                new SAV_BerryField(this, SaveGame.BerryField).ShowDialog();
        }
        private void B_OpenEventFlags_Click(object sender, EventArgs e)
        {
            // Open Flag Menu
            if (savegame_oras) 
                new SAV_EventFlagsORAS(this).ShowDialog();
            else 
                new SAV_EventFlagsXY(this).ShowDialog();
        }
        private void B_OpenSuperTraining_Click(object sender, EventArgs e)
        {
            // Open ST Menu
            new SAV_SuperTrain(this).ShowDialog();
        }
        private void B_OpenOPowers_Click(object sender, EventArgs e)
        {
            // Open O-Power Menu
            if (savegame_oras)
            {
                DialogResult dr = Util.Prompt(MessageBoxButtons.YesNo, "No editing support for ORAS :(", "Max O-Powers with a working code?");
                if (dr == DialogResult.Yes)
                {
                    byte[] maxoras = new byte[] 
                    { 
                        0x00, 0x01, 0x01, 0x01,
                        0x01, 0x00, 0x01, 0x01,
                        0x01, 0x01, 0x00, 0x01,
                        0x01, 0x01, 0x01, 0x00,
                        0x01, 0x01, 0x01, 0x01,
                        0x00, 0x01, 0x01, 0x01,
                        0x01, 0x00, 0x01, 0x01,
                        0x01, 0x01, 0x01, 0x01,
                        0x01, 0x01, 0x01, 0x01,
                        0x01, 0x01, 0x00, 0x01,
                        0x01, 0x01, 0x01, 0x00,
                        0x01, 0x01, 0x01, 0x01,
                        0x01, 0x01, 0x01, 0x01,
                        0x01, 0x01, 0x01, 0x01,
                        0x01, 0x01, 0x01, 0x01,
                        0x01, 0x01, 0x01, 0x01,
                        0x01, 0x00, 0x00, 0x00, 
                    };
                    Array.Copy(maxoras, 0, savefile, 0x17400 + 0x5400 + 0x7F000 * savindex, 0x44);
                }
            }
            else 
                new SAV_OPower(this).ShowDialog();
        }
        private void B_OpenPokedex_Click(object sender, EventArgs e)
        {
            // Open Pokedex Menu
            if (savegame_oras) 
                new SAV_PokedexORAS(this).ShowDialog();
            else 
                new SAV_PokedexXY(this).ShowDialog();
        }
        private void B_OUTPasserby_Click(object sender, EventArgs e)
        {
            string result = "";
            result += "PSS List" + Environment.NewLine + Environment.NewLine;
            string[] headers = {
                                   "PSS Data - Friends",
                                   "PSS Data - Acquaintances",
                                   "PSS Data - Passerby",
                               };
            int offset = savindex * 0x7F000 + SaveGame.PSS;
            for (int g = 0; g < 3; g++)
            {
                result += "----" + Environment.NewLine + headers[g] + Environment.NewLine + "----" + Environment.NewLine + Environment.NewLine;
                uint count = BitConverter.ToUInt32(savefile,offset + 0x4E20);
                int r_offset = offset;

                for (int i = 0; i < 100; i++)
                {
                    ulong unkn = BitConverter.ToUInt64(savefile, r_offset);
                    if (unkn == 0) break;

                    string otname = Util.TrimFromZero(Encoding.Unicode.GetString(savefile, r_offset + 8, 0x1A));
                    string message = Util.TrimFromZero(Encoding.Unicode.GetString(savefile, r_offset + 0x22, 0x22));

                    // Trim terminated

                    uint unk1 = BitConverter.ToUInt32(savefile, r_offset + 0x44);
                    ulong unk2 = BitConverter.ToUInt64(savefile, r_offset + 0x48);
                    uint unk3 = BitConverter.ToUInt32(savefile, r_offset + 0x50);
                    uint unk4 = BitConverter.ToUInt16(savefile, r_offset + 0x54);
                    byte region = savefile[r_offset + 0x56];
                    byte country = savefile[r_offset + 0x57];
                    byte _3dsreg = savefile[r_offset + 0x58];
                    byte _lang = savefile[r_offset + 0x59];
                    byte game = savefile[r_offset + 0x5A];
                    ulong outfit = BitConverter.ToUInt64(savefile, r_offset + 0x5C);
                    int favpkm = BitConverter.ToUInt16(savefile, r_offset + 0x9C) & 0x7FF;
                    string gamename;
                    if (game == 24)
                        gamename = "X";
                    else if (game == 25)
                        gamename = "Y";
                    else if(game == 26)
                        gamename = "AS";
                    else if (game == 27)
                        gamename = "OR";
                    else gamename = "UNK GAME";
                    result += 
                        "OT: " + otname + Environment.NewLine +
                        "Message: " + message + Environment.NewLine +
                        "Game: " + gamename + Environment.NewLine +
                        "Country ID: " + country + Environment.NewLine + 
                        "Region ID: " + region + Environment.NewLine +
                        "Favorite: " + specieslist[favpkm] + Environment.NewLine;

                    result += Environment.NewLine;
                    r_offset += 0xC8;
                }
                offset += 0x5000;
            }
            RTB_T.Text = result;
            RTB_T.Font = new Font("Courier New", 8);
            tabBoxMulti.SelectedIndex = 3;
        }
        private void B_OUTHallofFame_Click(object sender, EventArgs e)
        {
            // Open HoF Menu
            new SAV_HallOfFame(this).ShowDialog();
        }
        private void B_OpenSecretBase_Click(object sender, EventArgs e)
        {
            // Open Secret Base Menu
            new SAV_SecretBase(this).ShowDialog();;
        }

        private void B_OpenTemp_Click(object sender, EventArgs e)
        {
            string path;
            if (ModifierKeys == Keys.Control)
            {
                path = Util.GetCacheFolder();
                if (Directory.Exists(path))
                    System.Diagnostics.Process.Start("explorer.exe", @path);
                else
                    Util.Alert("Can't find the cache folder.");
            }
            else
            {
                path = Util.GetTempFolder();
                if (Directory.Exists(Path.Combine(path, "root")))
                    System.Diagnostics.Process.Start("explorer.exe", @Path.Combine(path, "root"));
                else if (Directory.Exists(path))
                    System.Diagnostics.Process.Start("explorer.exe", @path);
                else { Util.Error("Can't find the temporary file.", "Make sure the Cyber Gadget software is paused."); }
            }
        }
        private void B_SwitchSAV_Click(object sender, EventArgs e)
        {
            DialogResult switchsav = Util.Prompt(MessageBoxButtons.YesNo, String.Format("Current Savefile is Save {0}.",(savindex + 1)),String.Format("Would you like to switch to Save {0}?", ((savindex + 1) % 2 + 1)));
            if (switchsav == DialogResult.Yes)
            {
                savindex = (savindex + 1) % 2;
                setBoxNames();
                setPKXBoxes();
                setSAVLabel();
            }
        }
        private void B_JPEG_Click(object sender, EventArgs e)
        {
            int offset = 0x7F000 * savindex + SaveGame.JPEG;

            string filename = Encoding.Unicode.GetString(savefile, offset + 0, 0x1A).Replace("\0", string.Empty);
            filename += "'s picture";
            offset += 0x54;
            if (savefile[offset] != 0xFF)
            {
                Util.Alert("No PGL picture data found in the save file!");
                return;
            }
            int length = 0xE004;

            byte[] jpeg = new Byte[length];
            Array.Copy(savefile, offset, jpeg, 0, length);
            SaveFileDialog savejpeg = new SaveFileDialog();
            savejpeg.FileName = filename;
            savejpeg.Filter = "JPEG|*.jpeg";
            if (savejpeg.ShowDialog() == DialogResult.OK)
            {
                string path = savejpeg.FileName;
                if (File.Exists(path))
                {
                    // File already exists, save a .bak
                    byte[] backupfile = File.ReadAllBytes(path);
                    File.WriteAllBytes(path + ".bak", backupfile);
                }
                File.WriteAllBytes(path, jpeg);
            }
        }
        private void B_BoxIO_Click(object sender, EventArgs e)
        {
            // Allow Import/Export of Boxes
            new PKHeX.SAV_BoxIO(this, SaveGame.Box, SaveGame.PCLayout).ShowDialog();
        }        
        #endregion

        // Language Translation
        private void changeMainLanguage(object sender, EventArgs e)
        {
            if (init)
                buff = preparepkx(buff); // get data currently in form

            Menu_Options.DropDown.Close();
            InitializeStrings();
            InitializeLanguage();
            Util.TranslateInterface("Form1", curlanguage, Controls, menuStrip1);
            populateFields(buff); // put data back in form
        }
        private void InitializeStrings()
        {
            string[] lang_val = { "en", "ja", "fr", "it", "de", "es", "ko", "zh" };
            curlanguage = lang_val[CB_MainLanguage.SelectedIndex];
            
            string l = curlanguage;
            natures = Util.getStringList("Natures", l);
            types = Util.getStringList("Types", l);
            abilitylist = Util.getStringList("Abilities", l);
            movelist = Util.getStringList("Moves", l);
            itemlist = Util.getStringList("Items", l);
            characteristics = Util.getStringList("Character", l);
            specieslist = Util.getStringList("Species", l);
            wallpapernames = Util.getStringList("Wallpaper", l);
            itempouch = Util.getStringList("ItemPouch", l);

            if ((l != "zh") || (l == "zh" && !init)) // load initial binaries
            {
                forms = Util.getStringList("Forms", l);
                memories = Util.getStringList("Memories", l);
                genloc = Util.getStringList("GenLoc", l);
                trainingbags = Util.getStringList("TrainingBag", l);
                trainingstage = Util.getStringList("SuperTraining", l);
                puffs = Util.getStringList("Puff", l);
            }

            // Fix Item Names (Duplicate entries)
            itemlist[456] += " (OLD)"; // S.S. Ticket
            itemlist[463] += " (OLD)"; // Storage Key
            itemlist[478] += " (OLD)"; // Basement Key
            itemlist[626] += " (2)"; // Xtransceiver
            itemlist[629] += " (2)"; // DNA Splicers
            itemlist[637] += " (2)"; // Dropped Item
            itemlist[707] += " (2)"; // Travel Trunk
            itemlist[713] += " (2)"; // Alt Bike
            itemlist[714] += " (2)"; // Holo Caster
            itemlist[729] += " (1)"; // Meteorite
            itemlist[740] += " (2)"; // Contest Costume
            itemlist[751] += " (2)"; // Meteorite
            itemlist[771] += " (3)"; // Meteorite
            itemlist[772] += " (4)"; // Meteorite

            // Get the Egg Name and then replace it with --- for the comboboxes.
            eggname = specieslist[0];
            specieslist[0] = "---";

            // Get the met locations... for all of the games...
            metHGSS_00000 = Util.getStringList("hgss_00000", l);
            metHGSS_02000 = Util.getStringList("hgss_02000", l);
            metHGSS_03000 = Util.getStringList("hgss_03000", l);

            metBW2_00000 = Util.getStringList("bw2_00000", l);
            metBW2_30000 = Util.getStringList("bw2_30000", l);
            metBW2_40000 = Util.getStringList("bw2_40000", l);
            metBW2_60000 = Util.getStringList("bw2_60000", l);

            metXY_00000 = Util.getStringList("xy_00000", l);
            metXY_30000 = Util.getStringList("xy_30000", l);
            metXY_40000 = Util.getStringList("xy_40000", l);
            metXY_60000 = Util.getStringList("xy_60000", l);

            // Fix up some of the Location strings to make them more descriptive:
            metHGSS_02000[1] += " (NPC)";         // Anything from an NPC
            metHGSS_02000[2] += " ("+eggname+")"; // Egg From Link Trade
            metBW2_00000[36] = metBW2_00000[84] + "/" + metBW2_00000[36]; // Cold Storage in BW = PWT in BW2

            // BW2 Entries from 76 to 105 are for Entralink in BW
            for (int i = 76; i < 106; i++)
                metBW2_00000[i] = metBW2_00000[i] + "●";

            // Localize the Poketransfer to the language (30001)
            string[] ptransp = { "Poké Transfer", "ポケシフター", "Poké Fret", "Pokétrasporto", "Poképorter", "Pokétransfer", "포케시프터", "ポケシフター" };
            metBW2_30000[1-1] = ptransp[CB_MainLanguage.SelectedIndex];
            metBW2_30000[2-1] += " (NPC)";              // Anything from an NPC
            metBW2_30000[3-1] += " (" + eggname + ")";  // Egg From Link Trade

            // Zorua/Zoroark events
            metBW2_30000[10-1] = specieslist[251] + " (" + specieslist[570] + " 1)"; // Celebi's Zorua Event
            metBW2_30000[11-1] = specieslist[251] + " (" + specieslist[570] + " 2)"; // Celebi's Zorua Event
            metBW2_30000[12-1] = specieslist[571] + " (" + "1)"; // Zoroark
            metBW2_30000[13-1] = specieslist[571] + " (" + "2)"; // Zoroark

            metBW2_60000[3-1] += " (" + eggname + ")";  // Egg Treasure Hunter/Breeder, whatever...

            metXY_00000[104] += " (X/Y)";              // Victory Road
            metXY_00000[298] += " (OR/AS)";            // Victory Road
            metXY_30000[0] += " (NPC)";                // Anything from an NPC
            metXY_30000[1] += " (" + eggname + ")";    // Egg From Link Trade

            // Set the first entry of a met location to "" (nothing)
            // Fix (None) tags
            abilitylist[0] = itemlist[0] = movelist[0] = metXY_00000[0] = metBW2_00000[0] = metHGSS_00000[0] = "(" + itemlist[0] + ")";

            // Force an update to the met locations
            origintrack = "";

            if (init)
                updateIVs(null, null); // Prompt an update for the characteristics
        }

        // Drag & Drop within Box
        private void pbBoxSlot_MouseDown(object sender, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control || ModifierKeys == Keys.Alt || ModifierKeys == Keys.Shift || ModifierKeys == (Keys.Control | Keys.Alt))
            { slotModifier_Click(sender, (EventArgs)e); return; }
            PictureBox pb = (PictureBox)(sender);
            if (pb.Image == null)
                return;

            int slot = getSlot(sender);
            int offset = getPKXOffset(slot);
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                // Create Temp File to Drag
                string basepath = System.Windows.Forms.Application.StartupPath;
                Cursor.Current = Cursors.Hand;

                // Prepare Data
                Array.Copy(savefile, offset, pkm_from, 0, 0xE8);
                pkm_from_offset = offset;

                // Make a new file name based off the PID
                byte[] dragdata = PKX.decryptArray(pkm_from);
                Array.Resize(ref dragdata, 0xE8);
                PKX pkx = new PKX(dragdata);
                string filename = pkx.Nickname;
                if (filename != pkx.Species)
                    filename += " (" + pkx.Species + ")";
                filename += " - " + pkx.PID + ".pk6";

                // Make File
                string newfile = Path.Combine(basepath, Util.CleanFileName(filename));
                try
                {
                    File.WriteAllBytes(newfile, dragdata);

                    string[] filesToDrag = { newfile };
                    (sender as PictureBox).DoDragDrop(new DataObject(DataFormats.FileDrop, filesToDrag), DragDropEffects.Move);
                    File.Delete(newfile); // after drop, delete the temporary file
                }
                catch (ArgumentException x)
                { Util.Error("Drag & Drop Error:", x.ToString()); }
                File.Delete(newfile);
                pkm_from_offset = 0;
            }
        }
        private void pbBoxSlot_DragDrop(object sender, DragEventArgs e)
        {
            PictureBox pb = (PictureBox)(sender);
            int slot = getSlot(sender);
            int offset = getPKXOffset(slot);

            // Check for In-Dropped files (PKX,SAV,ETC)
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && pkm_from_offset == 0)
            {
                if (files.Length > 0)
                {
                    FileInfo fi = new FileInfo(files[0]);

                    // Detect if PKM/PKX
                    if ((fi.Length == 136) || (fi.Length == 220) || (fi.Length == 236) || (fi.Length == 100) || (fi.Length == 80))
                    {
                        byte[] input = File.ReadAllBytes(files[0]);
                        var Converter = new pk2pk();
                        if (!PKX.verifychk(input)) Util.Alert("Invalid File Loaded.", "Checksum is not valid.");
                        try // to convert past gen pkm
                        {
                            byte[] data = Converter.ConvertPKM(input, savefile, savindex);
                            Array.Copy(PKX.encryptArray(data), 0, savefile, offset, 0xE8);
                        }
                        catch
                        { Util.Error("Attempted to load previous generation PKM.", "Conversion failed."); }
                    }
                    else if (fi.Length == 232 || fi.Length == 260)
                    {
                        byte[] data = File.ReadAllBytes(files[0]);
                        if (fi.Extension == ".pkx" || fi.Extension == ".pk6")
                            data = PKX.encryptArray(data);
                        else if (fi.Extension != ".ekx" || fi.Extension != ".ekx")
                        { openQuick(files[0]); return; } // lazy way of aborting 

                        Array.Copy(data, 0, savefile, offset, 0xE8);
                    }
                    else // not PKX/EKX, so load with the general function
                    { openQuick(files[0]); }
                }
            }
            else
            {
                if (ModifierKeys == Keys.Alt)
                    Array.Copy(PKX.encryptArray(new Byte[0xE8]), 0, savefile, pkm_from_offset, 0xE8);
                else if (ModifierKeys != Keys.Control)
                    Array.Copy(savefile, offset, savefile, pkm_from_offset, 0xE8); // Copy from new slot to old slot.
                Array.Copy(pkm_from, 0, savefile, offset, 0xE8); // Copy from temp slot to new.
                pkm_from_offset = 0; // Clear offset value
            }
            setPKXBoxes();
        }
        private void pbBoxSlot_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null)
                e.Effect = DragDropEffects.Move;
        }
        private byte[] pkm_from = PKX.encryptArray(new Byte[0xE8]);
        private int pkm_from_offset = 0;
        private void L_Save_Click(object sender, EventArgs e)
        {
            // Get latest SaveDataFiler save location
            SDFLoc = Util.GetSDFLocation();
            string path = null;

            if (SDFLoc != null && ModifierKeys != Keys.Control) // if we have a result
                path = Path.Combine(SDFLoc, "main");
            else if (File.Exists(Util.NormalizePath(Path.Combine(Util.GetTempFolder(), "root" + Path.DirectorySeparatorChar + "main")))) // else if cgse exists
                path = Util.NormalizePath(Path.Combine(Util.GetTempFolder(), "root" + Path.DirectorySeparatorChar + "main"));

            if (path != null)
            {
                if (Util.Prompt(MessageBoxButtons.YesNo, "Open save file from the following location?", path) == DialogResult.Yes)
                    openQuick(path); // load save
            }
        }

        bool unicode = false;
        private void Menu_Unicode_Click(object sender, EventArgs e)
        {
            unicode = (gendersymbols[0] == "♂");
            if (unicode)
            {
                gendersymbols = new string[] { "M", "F", "-" };
                BTN_Shinytize.Text = "*";
            }
            else
            {
                gendersymbols = new string[] { "♂", "♀", "-" };
                BTN_Shinytize.Text = "☆";
            }
        }
    }
    #region Structs & Classes
    public class cbItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
    public class PersonalParser
    {
        public byte[] file = (byte[])Properties.Resources.ResourceManager.GetObject("personal");
        public int EntryLength = 0xE;
        public struct Personal
        {
            public byte[] BaseStats;
            public byte[] Abilities;
            public byte BaseFriendship;
            public byte GenderRatio;
            public byte EXPGrowth;
            public byte AltFormCount;
            public byte FormPointer; //721+FormPointer+(FormID-1)=SpeciesIndex           
        }

        public Personal GetPersonal(int species)
        {
            Personal data = new Personal();
            byte[] MonData = new byte[EntryLength];
            data.BaseStats = new byte[6];
            data.Abilities = new byte[3];
            Array.Copy(file, species * EntryLength, MonData, 0, EntryLength);
            Array.Copy(MonData, data.BaseStats, 6);
            Array.Copy(MonData, 6, data.Abilities, 0, 3);
            data.BaseFriendship = MonData[0x9];
            data.GenderRatio = MonData[0xA];
            data.EXPGrowth = MonData[0xB];
            data.AltFormCount = MonData[0xC];
            data.FormPointer = MonData[0xD];
            return data;
        }

        public Personal GetPersonal(int species, int formID)
        {
            Personal data = GetPersonal(species);
            if (formID > 0 && formID<=data.AltFormCount && data.AltFormCount>0 && data.FormPointer > 0) //Working with an Alt Forme with a base stat change
            {
                formID--;
                data = GetPersonal(721 + formID + data.FormPointer);
            }
            return data;
        }
    }
    public class SaveGames
    {
        public struct SaveStruct
        {
            public int Box, TrainerCard, Party, BattleBox, GTS, Daycare, Fused, SUBE, Puff, Item, Trainer1, Trainer2, PCLayout, Wondercard, BerryField, OPower, EventFlag, PokeDex, HoF, PSS, JPEG;
            public string Name;
            public SaveStruct(string GameID)
            {
                if (GameID == "XY")
                {
                    Name = "XY";
                    Box = 0x27A00;
                    TrainerCard = 0x19400;
                    Party = 0x19600;
                    BattleBox = 0x09E00;
                    Daycare = 0x20600;
                    GTS = 0x1CC00;
                    Fused = 0x1B400;
                    SUBE = 0x22C90;

                    Puff = 0x5400;
                    Item = 0x5800;
                    Trainer1 = 0x6800;
                    Trainer2 = 0x9600;
                    PCLayout = 0x9800;
                    Wondercard = 0x21000;
                    BerryField = 0x20C00;
                    OPower = 0x1BE00;
                    EventFlag = 0x19E00;
                    PokeDex = 0x1A400;

                    HoF = 0x1E800;
                    JPEG = 0x5C600;
                    PSS = 0x0A400;
                }
                else if (GameID == "ORAS")
                {
                    // Temp
                    Name = "ORAS";
                    Box = 0x38400;      // Confirmed
                    TrainerCard = 0x19400; // Confirmed
                    Party = 0x19600;    // Confirmed
                    BattleBox = 0x09E00;// Confirmed
                    Daycare = 0x21000; // Confirmed (thanks Rei)
                    GTS = 0x1D600; // Confirmed
                    Fused = 0x1BE00; // Confirmed
                    SUBE = 0x22C90; // ****not in use, not updating?****

                    Puff = 0x5400; // Confirmed
                    Item = 0x5800; // Confirmed
                    Trainer1 = 0x6800; // Confirmed
                    Trainer2 = 0x9600; // Confirmed
                    PCLayout = 0x9800; // Confirmed
                    Wondercard = 0x22000; // Confirmed
                    BerryField = 0x20C00; // ****changed****
                    OPower = 0x1BE00;
                    EventFlag = 0x19E00; // Confirmed
                    PokeDex = 0x1A400;

                    HoF = 0x1F200; // Confirmed
                    JPEG = 0x6D000; // Confirmed
                    PSS = 0x0A400; // Confirmed (thanks Rei)
                }
                else
                {
                    // Copied...
                    Name = "Unknown";
                    Box = 0x27A00;
                    TrainerCard = 0x19400;
                    Party = 0x19600;
                    BattleBox = 0x09E00;
                    Daycare = 0x20600;
                    GTS = 0x1CC00;
                    Fused = 0x1B400;
                    SUBE = 0x22C90;

                    Puff = 0x5400;
                    Item = 0x5800;
                    Trainer1 = 0x6800;
                    Trainer2 = 0x9600;
                    PCLayout = 0x9800;
                    Wondercard = 0x21000;
                    BerryField = 0x20C00;
                    OPower = 0x1BE00;
                    EventFlag = 0x19E00;
                    PokeDex = 0x1A400;

                    HoF = 0x1E800;
                    JPEG = 0x5C600;
                    PSS = 0x0A400;
                }
            }
        }
    }
    #endregion
}