using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Memory;
using System.Collections;
using System.Drawing.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RememberMe1
{

    public partial class Form1 : Form
    {
        Mem m = new Mem();
        public string curCheckpoint = "";
        public int isInCineamatic = 0;

        string SaveFileLocator = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public Form1()
        {
            InitializeComponent();
            CheckKeys();
        }

        private void CheckKeys()
        {
            string[] files = { "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "T", "Y", "U", "I", "O", "P", "G", "H", "J", "K", "L", "V", "B", "N", "M", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            string SaveFileBuffer = SaveFileLocator + @"\My Games\UnrealEngine3\RememberMeGame\Config";
            string SaveFilePath = SaveFileBuffer + "\\" + "ExampleInput.ini";
            if (!File.Exists(SaveFilePath))
            {
                MessageBox.Show("ExampleInput.ini file in folder " + SaveFileLocator + @"\My Games\UnrealEngine3\RememberMeGame\Config" + " was not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
                //throw new Exception("No config files");
            }
            var ini = File.ReadLines(SaveFilePath);
            string hotkey = "";

            foreach (string file in files)
            {
                if (!CancelText.Items.Contains(file))
                {
                    CancelText.Items.Add(file);
                }

                if (!SloMoText.Items.Contains(file))
                {
                    SloMoText.Items.Add(file);
                }
            }

            foreach (string line in ini) {
                if(line.Contains("Command=\"faster\"")) {
                    hotkey = line.Substring(line.IndexOf("=\"")+2, line.IndexOf("\",") - line.IndexOf("=\"")-2);
                    if(Array.IndexOf(files, hotkey) >= 0) {
                        SloMoText.Text = hotkey;
                        SloMoText.SelectedIndex = SloMoText.FindStringExact(hotkey);
                    }
                } else if(line.Contains("CANCELMATINEE")) {
                    hotkey = line.Substring(line.IndexOf("=\"")+2, line.IndexOf("\",") - line.IndexOf("=\"")-2);
                    if(Array.IndexOf(files, hotkey) >= 0) {
                        CancelText.Text = hotkey;
                        CancelText.SelectedIndex = CancelText.FindStringExact(hotkey);
                    }
                }
            }
        }

        private void ConfigChecker()
        {
            string SaveFileBuffer = SaveFileLocator + @"\My Games\UnrealEngine3\RememberMeGame\Config";
            string SaveFilePath = SaveFileBuffer + "\\" + "ExampleInput.ini";

            if (!File.Exists(SaveFilePath))
            {
                // InsertLine = true;
                MessageBox.Show("ExampleInput.ini file in folder " + SaveFileLocator + @"\My Games\UnrealEngine3\RememberMeGame\Config" + " was not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
                //throw new Exception("No config files");
            }

            if (SloMoText.SelectedIndex == -1 && CancelText.SelectedIndex == -1) {
                MessageBox.Show("No hotkeys were chosen", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
                //throw new Exception("No hotkeys chosen");
            } else if (SloMoText.SelectedIndex == -1 || CancelText.SelectedIndex == -1) {
                MessageBox.Show("Please choose both hotkeys", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string Spath = Path.GetDirectoryName(Application.StartupPath);
            Spath = Environment.ExpandEnvironmentVariables(Spath);
            var tempFile = Spath + @"\Temp Files\ExampleInput.ini";
            int sectionCount = 0;
            var linesToKeep = File.ReadLines(SaveFilePath).Where(l => !l.Contains("faster")).Where(l => !l.Contains(SloMoText.Items[SloMoText.SelectedIndex].ToString()+",")).Where(l => !l.Contains(CancelText.Items[CancelText.SelectedIndex].ToString()+",")).Where(l => !l.Contains("CANCELMATINEE")).Where(l => !l.Contains("set SeqAct_Interp Playrate"));
            File.WriteAllLines(tempFile, linesToKeep);

            using (StreamWriter sw = File.AppendText(tempFile))
            {
                sw.WriteLine("[Engine.PlayerInput]\r\n" + "Bindings=(Name=\"" + SloMoText.Items[SloMoText.SelectedIndex].ToString() + "\",Command=\"faster\")\r\nBindings=(Name=\"faster\",Command=\"set SeqAct_Interp Playrate 100 | setbind " + SloMoText.Items[SloMoText.SelectedIndex].ToString() + " slower\")\r\nBindings=(Name=\"slower\",Command=\"set SeqAct_Interp Playrate 1 | setbind " + SloMoText.Items[SloMoText.SelectedIndex].ToString() + " faster\")");
                sw.WriteLine("Bindings=(Name=\"" + CancelText.Items[CancelText.SelectedIndex].ToString() + "\",Command=\"CANCELMATINEE\")");
            }

            File.Delete(SaveFilePath);
            File.Copy(tempFile, SaveFilePath);
            File.SetAttributes(SaveFilePath, FileAttributes.Normal);

            if(Process.GetProcessesByName("RememberMe").Length > 0) {
                MessageBox.Show("Please restart the game for the new config to take effect.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SendKeyCan()
        {
            [DllImport("User32.dll")]
            static extern int SetForegroundWindow(IntPtr point);
            [DllImport("user32.dll")]
            static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

            Process p = Process.GetProcessesByName("RememberMe").FirstOrDefault();
            if (p != null)
            {
                IntPtr h = p.MainWindowHandle;
                SetForegroundWindow(h);
                string hotkey = CancelText.Items[CancelText.SelectedIndex].ToString();
                short scanCode;

                scanCode = (short)Enum.Parse(typeof(ScanCodeShort), "Key_" + hotkey, true);
                
                INPUT[] input = new INPUT[1];

                input[0].type = 1;
                if(hotkey.Length == 1) input[0].U.ki.dwFlags = KEYEVENTF.SCANCODE | KEYEVENTF.UNICODE;
                else input[0].U.ki.dwFlags = KEYEVENTF.SCANCODE;
                input[0].U.ki.wScan = (ScanCodeShort)scanCode;
                SendInput(1, input, INPUT.Size);

                Thread.Sleep(100);

                input[0].type = 1;
                if(hotkey.Length == 1) input[0].U.ki.dwFlags = KEYEVENTF.SCANCODE | KEYEVENTF.UNICODE | KEYEVENTF.KEYUP;
                else input[0].U.ki.dwFlags = KEYEVENTF.SCANCODE | KEYEVENTF.KEYUP;
                input[0].U.ki.wScan = (ScanCodeShort)scanCode;
                SendInput(1, input, INPUT.Size);
                Console.WriteLine("Skip Inside Function");
            }
        }

        private void SendKeySlo()
        {

            [DllImport("User32.dll")]
            static extern int SetForegroundWindow(IntPtr point);
            [DllImport("user32.dll")]
            static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

            Process p = Process.GetProcessesByName("RememberMe").FirstOrDefault();
            if (p != null)
            {
                IntPtr h = p.MainWindowHandle;
                SetForegroundWindow(h);

                string hotkey = SloMoText.Items[SloMoText.SelectedIndex].ToString();
                Console.WriteLine(hotkey);
                short scanCode;

                scanCode = (short)Enum.Parse(typeof(ScanCodeShort), "Key_" + hotkey, true);
                INPUT[] input = new INPUT[1];

                input[0].type = 1;
                if(hotkey.Length == 1) input[0].U.ki.dwFlags = KEYEVENTF.SCANCODE | KEYEVENTF.UNICODE;
                else input[0].U.ki.dwFlags = KEYEVENTF.SCANCODE;
                input[0].U.ki.wScan = (ScanCodeShort)scanCode;
                SendInput(1, input, INPUT.Size);

                Thread.Sleep(100);

                input[0].type = 1;
                if(hotkey.Length == 1) input[0].U.ki.dwFlags = KEYEVENTF.SCANCODE | KEYEVENTF.UNICODE | KEYEVENTF.KEYUP;
                else input[0].U.ki.dwFlags = KEYEVENTF.SCANCODE | KEYEVENTF.KEYUP;
                input[0].U.ki.wScan = (ScanCodeShort)scanCode;
                SendInput(1, input, INPUT.Size);

                Console.WriteLine("Speed up Inside Function");
            }
        }

        [DllImport("User32.dll")]

        static extern short GetAsyncKeyState(Int32 vKey);

        int VK_DELETE = 0x2E;
        int VK_F1 = 0x70;
        bool procopen = false;
        public string oldString = string.Empty;
        List<string> doneSplit = new List<string>();	

        private async void timer1_Tick(object sender, EventArgs e)
        {
            procopen = m.OpenProcess("rememberme");
            if (!procopen)
            {
                //await Task.Delay(1000);
                return;
            }

            isInCineamatic = m.ReadInt("base+0x12432C0");
            curCheckpoint = m.ReadString("base+0x012700F0,0x160,0x3C,0x3C,0x88,0x0", "", 128, true, System.Text.Encoding.Unicode);

            textBox3.Text = curCheckpoint;
            textBox4.Text = isInCineamatic.ToString();

            /*short keyStatus = GetAsyncKeyState(VK_DELETE);
            short keySta = GetAsyncKeyState(VK_F1);

            if ((keySta & 1) == 1)
            {
                Close();
            }*/

            if (isInCineamatic == 1)
            {
                string Checkpointsaved = curCheckpoint;
                var (command, Time, TimesToRun) = _skips[curCheckpoint];

                textBox3.Text = command.ToString();
                
                for (int loop = 0; loop <= TimesToRun;)
                {
                    curCheckpoint = m.ReadString("base+0x012700F0,0x160,0x3C,0x3C,0x88,0x0", "", 128, true, System.Text.Encoding.Unicode);
                    isInCineamatic = m.ReadInt("base+0x12432C0");
                    textBox4.Text = isInCineamatic.ToString();

                    /*if (curCheckpoint != Checkpointsaved) {  // needs to handle cases where you exit while inside of a cutscene, but doesn't work because there are cases where you get a checkpoint before the cutscene is over, which is bad in this case
                        return;
                    }*/

                    if(isInCineamatic == 1) {
                        if (command == SkipCommand.Skip)
                        {
                            Thread.Sleep(Time+50); //for autosplitter
                            Console.WriteLine("Skip");
                            SendKeyCan();
                        }

                        if (command == SkipCommand.FastForward) // still problems in the game with the fastforwards sometimes, in which it stops the fastforwards halfway or doesn't stop fast forwarding, but this is what I have for now
                        {
                            Thread.Sleep(50); //for autosplitter
                            if (Time != 0)
                            {
                                Thread.Sleep(Time);
                                SendKeySlo();
                                Console.WriteLine("Speed up, Timed");
                                while(isInCineamatic == 1) {
                                    isInCineamatic = m.ReadInt("base+0x12432C0");
                                    //Console.WriteLine("Wait Fast Forward, Timed," + " Loop " + loop.ToString());
                                }
                            }
                            else 
                            {
                                SendKeySlo();
                                Console.WriteLine("Speed up, Untimed");
                                while(isInCineamatic == 1) {
                                    isInCineamatic = m.ReadInt("base+0x12432C0");
                                    //Console.WriteLine("Wait Fast Forward, Untimed," + " Loop " + loop.ToString());
                                }
                            }
                        }
                    }
                    else if (command == SkipCommand.FastForward && loop % 2 == 1) 
                    {
                        SendKeySlo();
                        Console.WriteLine("Speed reset");
                    }
                    else if (command == SkipCommand.FastForward && loop % 2 == 0) 
                    {
                        while(isInCineamatic == 0) {
                            isInCineamatic = m.ReadInt("base+0x12432C0");
                            //Console.WriteLine("Wait Fast Forward," + " Loop " + loop.ToString());
                        }
                        loop--;
                    }
                    Console.WriteLine(loop);
                    loop++;
                }
                //command = SkipCommand.Nothing;
               // doneSplit.Add(Checkpointsaved);

            }
        }

        // calls text updater
        private void CancelText_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        //Calls config checker
        private void button1_Click(object sender, EventArgs e)
        {
            ConfigChecker();
        }

        /*private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string Spath = Path.GetDirectoryName(Application.StartupPath);
            Spath = Environment.ExpandEnvironmentVariables(Spath);
            var tempFile = Spath + @"\Temp Files\SaveFile.txt";

            
                using (StreamWriter sw = File.AppendText(tempFile))
                {
                        sw.WriteLine(CancelText.SelectedIndex.ToString() + "Can");
                        sw.WriteLine(SloMoText.SelectedIndex.ToString() + "Slo");
                }

        }*/

        /*private void Form1_Load(object sender, EventArgs e)
        {
            string Spath = Path.GetDirectoryName(Application.StartupPath);
            Spath = Environment.ExpandEnvironmentVariables(Spath);
            var tempFile = Spath + @"\Temp Files\SaveFile.txt";


            var lines = File.ReadAllLines(tempFile);

            foreach (var line in lines)
            {
                if (line.Contains("Can"))
                {
                    line.Replace("Can", null);
                    CancelText.SelectedIndex = line.Replace("Can", null);
                }

                if (line.Contains("Slo"))
                {
                    line.Replace("Slo", null);
                    SloMoText.SelectedIndex = line.Replace("Slo", null);
                }
            }
            File.Copy(tempFile, Spath + @"\Temp Files\SaveFileCopy.txt", true);
            File.Delete(tempFile);
            using (StreamWriter sw = File.AppendText(tempFile))
            {
                if(CancelText != null) {
                    sw.WriteLine(CancelText.SelectedIndex.ToString() + "Can");
                    sw.WriteLine(SloMoText.SelectedIndex.ToString() + "Slo");
                }
            }
        }*/

        

        private void SloMoText_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            doneSplit.Clear();
        }

        enum SkipCommand { Skip, FastForward, Nothing };
        record struct CutsceneSkip(SkipCommand Command, int Time, float TimesToRun);

        static Dictionary<string, CutsceneSkip> _skips = new()
        {
            ["01_Intro"] = new(SkipCommand.Skip, 14000, 1),
            ["01b_Intro"] = new(SkipCommand.Skip, 2, 3),
            ["02_ZornChase"] = new(SkipCommand.FastForward, 1, 1),
            ["03_SanitationPit"] = new(SkipCommand.FastForward, 0, 1),
            ["E1_Begin"] = new(SkipCommand.FastForward,19500, 1),
            ["01_2_Checkpoint_MetroFight"] = new(SkipCommand.Skip, 0, 1),
            ["03_Checkpoint_AfterJunkYard"] = new(SkipCommand.FastForward, 0, 1),
            ["04_Checkpoint_BeginOldSouk"] = new(SkipCommand.Skip, 0, 1),
            ["08_Checkpoint_FloodgateDown"] = new(SkipCommand.FastForward, 0, 1),
            ["09_2_Checkpoint_WaveFightSlumDock"] = new(SkipCommand.FastForward, 0, 1),
            ["10_Checkpoint_AfterFightSlumDock"] = new(SkipCommand.FastForward, 0, 1),
            ["011_2_Checkpoint_BeginFloatingMarket(relookat, 1)"] = new(SkipCommand.Skip, 0, 1),
            ["12_3_Checkpoint_CorridorBeginInter"] = new(SkipCommand.FastForward, 0, 1),
            ["14_Checkpoint_CorridorEnd"] = new(SkipCommand.Nothing, 0, 0),
            ["15_Checkpoint_LeakingBrainBegin"] = new(SkipCommand.FastForward, 0, 1),
            ["16_Checkpoint_LeakingBrainRTC"] = new(SkipCommand.FastForward, 0, 1),
            ["MemoryRemix_03"] = new(SkipCommand.Nothing, 0, 1),
            ["01_BeginEpisode"] = new(SkipCommand.Skip, 0, 1),
            ["02a_Courtyard_Fight1_BeforeRTC"] = new(SkipCommand.FastForward, 0, 1),
            ["05a_Courtyard_Fight2_BeforeRTC"] = new(SkipCommand.Skip, 0, 1),
            ["05b_Courtyard_Fight2_BeforeFight"] = new(SkipCommand.Nothing, 0, 0),
            ["08_Canopy_Begin"] = new(SkipCommand.FastForward, 0, 1),
            ["09c_AfterCanopy_Fight_AfterFight"] = new(SkipCommand.FastForward, 0, 1),
            ["10b_ComfortressRoofs_Begin_AfterRTC"] = new(SkipCommand.Skip, 0, 1),
            ["10c_ComfortressRoofs_BadGhost3"] = new(SkipCommand.FastForward, 0, 1),
            ["12b_ComfortressRoofs1_Fight_BeforeFight"] = new(SkipCommand.FastForward, 0, 1),
            ["15b_ComfortressRoofs2_BadGhost5"] = new(SkipCommand.FastForward, 0, 1),
            ["18_ComfortressRoofs3_End"] = new(SkipCommand.Skip, 0, 1),
            ["19a_ComfortressFacade_HackKaori"] = new(SkipCommand.FastForward, 0, 1),
            ["20a_ComfortressFacade_End"] = new(SkipCommand.Skip, 0, 1),
            ["21_Kidroof_KidFight_1"] = new(SkipCommand.FastForward, 0, 1),
            ["22_Kidroof_KidFight_2"] = new(SkipCommand.FastForward, 0, 1),
            ["23b_Kidroof_KidFight_4"] = new(SkipCommand.Nothing, 0, 0),
            ["24_Kidroof_KidFight_End"] = new(SkipCommand.Skip, 0, 1),
            ["CP_Episode_Episode3_Begin"] = new(SkipCommand.Skip, 0, 1),
            ["CP_Map_SwitchRoom_BeforeFight"] = new(SkipCommand.FastForward, 0, 1),
            ["CP_Respawn_ThroughWindows_EnterStranglerHouse"] = new(SkipCommand.Skip, 0, 1),
            ["CP_Map_ThroughWindows_BeforeFight"] = new(SkipCommand.FastForward, 0, 1),
            ["CP_Respawn_Pillars_BeforeJump"] = new(SkipCommand.FastForward, 0, 1),
            ["CP_Map_InHouseFight_BeforeFight"] = new(SkipCommand.Skip, 0, 1),
            ["CP_Map_InHouseFight_AfterFight"] = new(SkipCommand.FastForward, 0, 1),
            ["CP_Map_InHouseFight_BeforeJump"] = new(SkipCommand.FastForward, 0, 1),
            ["CP_Map_Dam_BeforeFight"] = new(SkipCommand.FastForward, 0, 1),
            ["CP_Respawn_SinkCrown_JunkBoltOk"] = new(SkipCommand.Skip, 0, 1),
            ["CP_Respawn_SinkCrown_JunkBoltGiven"] = new(SkipCommand.FastForward, 0, 1),
            ["CP_Map_CanalCorridor_FightFinished"] = new(SkipCommand.Skip, 0, 1),
            ["CP_Map_MetroHack_Entrance"] = new(SkipCommand.FastForward, 0, 1),
            ["CP_Respawn_MetroExit_BeforeFight"] = new(SkipCommand.FastForward, 0, 1),
            ["CP_Map_SunkStation_Exit"] = new(SkipCommand.FastForward, 0, 1),
            ["CP_Map_BastilleAccess_MinesPassed"] = new(SkipCommand.FastForward, 0, 1),
            ["CP_Respawn_Bastille_MinesSecondPart"] = new(SkipCommand.FastForward, 0, 1),
            ["CP_Respawn_Bastille_ZornDead"] = new(SkipCommand.Skip, 0, 1),
            ["LevelBegin"] = new(SkipCommand.Skip, 0, 1),
            ["E4_Conveyor_EnteringSector"] = new(SkipCommand.FastForward, 0, 1),
            ["PR1CheckPoint"] = new(SkipCommand.FastForward, 0, 1),
            ["PrisonRoom1_AfterEdgeCall"] = new(SkipCommand.Nothing, 0, 0),
            ["AdmCorridors_BeforeCine"] = new(SkipCommand.Skip, 0, 1),
            ["LockerRoom_BeforeEntering"] = new(SkipCommand.Nothing, 0, 0),
            ["SpartansRoom_BeforeHeavyEnforcer"] = new(SkipCommand.Skip, 0, 1),
            ["PrisonRoom2_EnteringSector"] = new(SkipCommand.Skip, 0, 1),
            ["IntersticeToSera_EnteringSector"] = new(SkipCommand.FastForward, 0, 1),
            ["SeraphimRoom_BeforeFight"] = new(SkipCommand.Nothing, 0, 1),
            ["SeraToMadam_BeforeInterrogationRoom"] = new(SkipCommand.FastForward, 0, 1),
            ["Courtyard_BeforeFight"] = new(SkipCommand.FastForward, 0, 1),
            ["E4_FightCheckpoint_Courtyard_AfterfirstWave"] = new(SkipCommand.Nothing, 0, 0),
            ["ServerRoom_EnteringSector"] = new(SkipCommand.FastForward, 0, 1),
            ["CP_AfterIntro"] = new(SkipCommand.FastForward, 0, 1),
            ["CP01"] = new(SkipCommand.FastForward, 0, 1),
            ["ServerRoom_BackFromEgoRoom"] = new(SkipCommand.Skip, 0, 1),
            ["MemoryRemix_02"] = new(SkipCommand.Skip, 0, 1),
            ["E5_Begin"] = new(SkipCommand.Skip, 0, 1),
            ["E5_LeaperAmbush_FightEnd"] = new(SkipCommand.FastForward, 0, 1),
            ["E5_FloodedStreet"] = new(SkipCommand.Skip, 0, 1),
            ["E5_NephilimEncounter"] = new(SkipCommand.Nothing, 0, 0),
            ["E5_MemoreyesHQ"] = new(SkipCommand.FastForward, 0, 1),
            ["E5_TraceFirstView"] = new(SkipCommand.Skip, 0, 1),
            ["E5_GroundBreakAlley"] = new(SkipCommand.Nothing, 0, 0),
            ["E5_EdgeReminder_AfterCin"] = new(SkipCommand.FastForward, 0, 1),
            ["E5_TraceHacked"] = new(SkipCommand.FastForward, 0, 1),
            ["E5_ReturnToMemHQ"] = new(SkipCommand.Nothing, 0, 0),
            ["E5_CorridorToScylla"] = new(SkipCommand.FastForward, 0, 1),
            ["E5_MRComplete"] = new(SkipCommand.Skip, 0, 1),
            ["00_IntermissionEp06"] = new(SkipCommand.Skip, 0, 1),
            ["02A_BlitzFight01"] = new(SkipCommand.Skip, 500, 1),
            ["03A_PrisonLockers_GettingPickSocket"] = new(SkipCommand.Skip, 0, 1),
            ["05_ExpRoomA_Reveal"] = new(SkipCommand.Skip, 0, 1),
            ["06_ExpRoomA_FightStart"] = new(SkipCommand.Skip, 0, 1),
            ["07_ExpRoomA_FightEnd"] = new(SkipCommand.Skip, 0, 1),
            ["09_Interstice_Start"] = new(SkipCommand.Skip, 0, 1),
            ["15_Laboratories_Ghost04"] = new(SkipCommand.Skip, 0, 1),
            ["16A_ExpB"] = new(SkipCommand.FastForward, 0, 1),
            ["20_QuaidsLab_FightStart"] = new(SkipCommand.FastForward, 0, 1),
            ["21_ToSanitation_FightEnd"] = new(SkipCommand.FastForward, 0, 1),
            ["22_SanitationPit_FightStart"] = new(SkipCommand.Skip, 0, 1),
            ["23_SanitationPit_FightEnd"] = new(SkipCommand.Skip, 0, 1),
            ["InitialLoad"] = new(SkipCommand.Skip, 0, 1),
            ["WarehouseFightEnd"] = new(SkipCommand.FastForward, 0, 1),
            ["PlatformZornBegin"] = new(SkipCommand.Nothing, 0, 0),
            ["04_PwEntry"] = new(SkipCommand.FastForward, 0, 1),
            ["07_PwWarehouse"] = new(SkipCommand.FastForward, 0, 1),
            ["LiftA"] = new(SkipCommand.FastForward, 0, 1),
            ["13_MallEntry"] = new(SkipCommand.FastForward, 0, 1),
            ["MallEntry2"] = new(SkipCommand.FastForward, 0, 1),
            ["17_MallBig"] = new(SkipCommand.Skip, 0, 1),
            ["18_MallBigAgility"] = new(SkipCommand.FastForward, 0, 1),
            ["19_MallArena"] = new(SkipCommand.FastForward, 0, 1),
            ["23_SecurityIn"] = new(SkipCommand.FastForward, 30, 1),
            ["24_SecurityOut"] = new(SkipCommand.FastForward, 0, 1),
            ["37_LuxuryEnd"] = new(SkipCommand.FastForward, 0, 1),
            ["LuxuryEndFightEnd"] = new(SkipCommand.FastForward, 0, 1),
            ["Episode8"] = new(SkipCommand.FastForward, 0, 1),
            ["CP_CharlesOffice_AgilityEnd"] = new(SkipCommand.Skip, 0, 1),
            ["MemoryRemix_04_Part1"] = new(SkipCommand.FastForward, 800, 1),
            ["MemoryRemix_04_Part2"] = new(SkipCommand.FastForward, 500, 1),
            ["MemoryRemix_04_Part3"] = new(SkipCommand.FastForward, 0, 1),
            ["CP_MR04_Finished"] = new(SkipCommand.FastForward, 1000, 1),
            ["CP_H3ORoom_beforeRTC"] = new(SkipCommand.FastForward, 0, 1),
            ["CP00_Ego2"] = new(SkipCommand.Skip, 0, 1),
            ["CP_BeginOlga"] = new(SkipCommand.Nothing, 0, 0),
            ["CP_BeginScylla"] = new(SkipCommand.FastForward, 0, 1),
        
        };
    }
}
