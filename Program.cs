using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Blocul17 {
public class GameWindow:Form {
 public readonly GameModel Model=new GameModel();
 readonly GameView view=new GameView();readonly Timer timer=new Timer();readonly Stopwatch clock=Stopwatch.StartNew();readonly HashSet<Keys> held=new HashSet<Keys>();
 readonly Bitmap frame=new Bitmap(GameView.Width,GameView.Height);readonly string savePath;
 public string Screen="intro";string returnScreen="";bool hasSave=false,saveFailed=false,smoke=false;int ticks;double previous;
 public int Frames;public bool PauseVerified=false,ResumeVerified=false,DialoguePauseVerified=false,SocialCommandsVerified=false,ShovelCommandVerified=false,StorageCommandVerified=false,StoragePauseVerified=false,WaitingCommandVerified=false,WaitingPauseVerified=false;double waitAt;double storageAt;double pausedAt,dialogueAt;
 public GameWindow(bool smokeTest=false){
  smoke=smokeTest;savePath=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"Blocul17","save-v4.xml");
  Text="Blocul 17 — o ieșire există";var area=System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;ClientSize=new Size(Math.Min(1280,area.Width-60),Math.Min(900,area.Height-80));MinimumSize=new Size(920,690);StartPosition=FormStartPosition.CenterScreen;BackColor=Color.FromArgb(20,37,40);DoubleBuffered=true;KeyPreview=true;SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.UserPaint|ControlStyles.OptimizedDoubleBuffer,true);
  if(!smoke){string legacy=Path.Combine(Path.GetDirectoryName(savePath),"save.xml");string prior=Path.Combine(Path.GetDirectoryName(savePath),"save-v2.xml");string prior3=Path.Combine(Path.GetDirectoryName(savePath),"save-v3.xml");string input=File.Exists(savePath)?savePath:File.Exists(prior3)?prior3:File.Exists(prior)?prior:legacy;hasSave=Model.Load(input);if(!hasSave&&File.Exists(input))Model.Tell("Salvarea este invalidă. Începi o partidă nouă; fișierul vechi va fi păstrat.");if(hasSave&&input!=savePath)Model.Tell("Partida anterioară a fost importată. Originalul rămâne păstrat.");}
  else{ShowInTaskbar=false;Opacity=0;}
  timer.Interval=16;timer.Tick+=OnTick;timer.Start();Deactivate+=delegate{Model.StopWaiting("Așteptare oprită: fereastra este în pauză.");held.Clear();if(Screen==""&&!Model.S.Won&&Model.S.Health>0)Screen="pause";};FormClosing+=delegate{if(!smoke)Save();};
 }
 bool Save(){try{if(!hasSave&&File.Exists(savePath))File.Copy(savePath,savePath+".invalid-"+DateTime.Now.ToString("yyyyMMddHHmmss"),false);Model.Save(savePath);hasSave=true;saveFailed=false;return true;}catch(Exception){saveFailed=true;Model.AutoSaveTime=0;Model.Tell("Salvarea nu a reușit. Verifică accesul la dosarul LocalAppData.");return false;}}
 void OnTick(object sender,EventArgs e){double now=clock.Elapsed.TotalSeconds,dt=Math.Min(.05,now-previous);previous=now;if(Screen==""){
  double mx=(Down(Keys.D)||Down(Keys.Right)?1:0)-(Down(Keys.A)||Down(Keys.Left)?1:0),my=(Down(Keys.S)||Down(Keys.Down)?1:0)-(Down(Keys.W)||Down(Keys.Up)?1:0);Model.Advance(dt,mx,my,Down(Keys.ShiftKey));if(!smoke&&Model.AutoSaveTime>=20)Save();}
  Invalidate();if(smoke){ticks++;if(ticks==2){Command("start");}if(ticks==5){ResumeVerified=Model.S.Minutes>420;Command("pause");pausedAt=Model.S.Minutes;}if(ticks==10)PauseVerified=Model.S.Minutes==pausedAt;if(ticks==11){Screen="";Model.S.X=Model.Inmates[0].X;Model.S.Y=Model.Inmates[0].Y;Model.S.Inventory.Add("biscuiți");Command("talk");Command("accept");dialogueAt=Model.S.Minutes;}if(ticks==14){DialoguePauseVerified=Screen=="talk"&&Model.S.Minutes==dialogueAt;Command("deliver");SocialCommandsVerified=Model.S.Money==18&&Model.S.ActiveFavor==-1;Command("close");}if(ticks==15){Model.S.Inventory.Clear();Model.S.Inventory.AddRange(new[]{"pilă","sârmă","bandă"});Model.S.Intellect=40;Command("craft");Command("make-shovel");ShovelCommandVerified=Screen=="craft"&&Model.S.Inventory.Count==1&&Model.S.Inventory[0]=="lopată";}if(ticks==17){Command("close");Model.S.X=5.5;Model.S.Y=5.5;Command("interact");Command("storage-toggle");Command("deposit");storageAt=Model.S.Minutes;}if(ticks==20){StoragePauseVerified=Screen=="storage"&&Model.S.Minutes==storageAt;bool stashed=Model.S.Stash.Count==1&&Model.S.Inventory.Count==0;Command("take0");StorageCommandVerified=stashed&&Model.S.Stash.Count==0&&Model.S.Inventory.Contains("lopată");}if(ticks==22){Command("close");Model.S.Minutes=730;Command("wait");waitAt=Model.S.Minutes;}if(ticks==25){WaitingCommandVerified=Model.Waiting&&Model.S.Minutes-waitAt>.2;Command("pause");waitAt=Model.S.Minutes;}if(ticks==29)WaitingPauseVerified=!Model.Waiting&&Model.S.Minutes==waitAt;if(ticks>=30)Close();}
 }
 bool Down(Keys key){return held.Contains(key);}
 protected override bool IsInputKey(Keys keyData){return true;}
 protected override void OnKeyDown(KeyEventArgs e){base.OnKeyDown(e);bool first=held.Add(e.KeyCode);e.SuppressKeyPress=true;if(!first)return;if(e.KeyCode!=Keys.T)Model.StopWaiting("Așteptare oprită.");
  if(e.KeyCode==Keys.F11){if(FormBorderStyle==FormBorderStyle.None){FormBorderStyle=FormBorderStyle.Sizable;WindowState=FormWindowState.Normal;}else{FormBorderStyle=FormBorderStyle.None;WindowState=FormWindowState.Maximized;}return;}
  if(Model.S.Won||Model.S.Health<=0){if(e.KeyCode==Keys.Enter)Command("restart");return;}
  if(e.KeyCode==Keys.F5){Command("save");return;}if(e.KeyCode==Keys.F1){Command("guide");return;}
  if(e.KeyCode==Keys.Escape){if(Screen=="intro")return;if(Screen=="")Command("pause");else Command("close");return;}
  if(Screen=="intro"&&e.KeyCode==Keys.Enter){Command("start");return;}if(Screen!=""){if(Screen=="storage"){if(e.KeyCode>=Keys.D1&&e.KeyCode<=Keys.D6)Command("take"+((int)e.KeyCode-(int)Keys.D1));if(e.KeyCode==Keys.H)Command("storage-toggle");if(e.KeyCode==Keys.V)Command("deposit");if(e.KeyCode==Keys.Left)Command("storage-slot-prev");if(e.KeyCode==Keys.Right)Command("storage-slot-next");if(e.KeyCode==Keys.PageUp)Command("storage-prev");if(e.KeyCode==Keys.PageDown)Command("storage-next");}if(Screen=="craft"){if(e.KeyCode==Keys.Enter||e.KeyCode==Keys.D1)Command("make");if(e.KeyCode==Keys.D2)Command("make-shovel");}if(Screen=="talk"){if(Model.TalkNpc==1){if(e.KeyCode>=Keys.D1&&e.KeyCode<=Keys.D4)Command("buy"+((int)e.KeyCode-(int)Keys.D1));if(e.KeyCode==Keys.Left)Command("sale-prev");if(e.KeyCode==Keys.Right)Command("sale-next");if(e.KeyCode==Keys.V)Command("sell");}else if(e.KeyCode==Keys.Enter)Command(Model.S.ActiveFavor==Model.FavorForNpc(Model.TalkNpc)?"deliver":"accept");}return;}
  if(e.KeyCode==Keys.T)Command("wait");if(e.KeyCode==Keys.E)Command("interact");if(e.KeyCode==Keys.F)Command("talk");if(e.KeyCode==Keys.C)Command("craft");if(e.KeyCode==Keys.J)Command("journal");
  if(e.KeyCode>=Keys.D1&&e.KeyCode<=Keys.D6){view.Selected=(int)e.KeyCode-(int)Keys.D1;Model.Use(view.Selected);}if(e.KeyCode==Keys.Q)Model.Drop(view.Selected);
 }
 protected override void OnKeyUp(KeyEventArgs e){held.Remove(e.KeyCode);base.OnKeyUp(e);}
 RectangleF ViewRect(){float s=Math.Min((float)ClientSize.Width/GameView.Width,(float)ClientSize.Height/GameView.Height);return new RectangleF((ClientSize.Width-GameView.Width*s)/2,(ClientSize.Height-GameView.Height*s)/2,GameView.Width*s,GameView.Height*s);}
 Point Logical(Point p){var r=ViewRect();return new Point((int)((p.X-r.X)*GameView.Width/r.Width),(int)((p.Y-r.Y)*GameView.Height/r.Height));}
 protected override void OnMouseMove(MouseEventArgs e){base.OnMouseMove(e);Point p=Logical(e.Location);view.Hover=view.Hit(p.X,p.Y);Cursor=view.Hover==""?Cursors.Default:Cursors.Hand;}
 protected override void OnMouseDown(MouseEventArgs e){base.OnMouseDown(e);Point p=Logical(e.Location);string id=view.Hit(p.X,p.Y);if(id.StartsWith("item")){Model.StopWaiting("Așteptare oprită.");int slot=int.Parse(id.Substring(4));view.Selected=slot;if(e.Button==MouseButtons.Right)Model.Drop(slot);else Model.Use(slot);}else Command(id);}
 public void Command(string id){if(id!=""&&id!="wait")Model.StopWaiting("Așteptare oprită.");if(id=="wait"&&Screen==""){Model.ToggleWait();held.Clear();}
  else if(id=="start"||id=="close"){Screen=id=="close"?returnScreen:"";returnScreen="";Model.TalkNpc=-1;Model.StorageCrate=-1;Model.HiddenStorage=false;held.Clear();}
  else if(id=="interact"&&Screen==""){Model.Interact();if(Model.StorageCrate>=0){Screen="storage";returnScreen="";view.Selected=0;view.StoragePage=0;held.Clear();}}
  else if(Screen=="storage"&&id=="storage-toggle"){if(Model.ToggleStorage())view.StoragePage=0;}
  else if(Screen=="storage"&&id.StartsWith("take")){int slot;if(int.TryParse(id.Substring(4),out slot)&&slot>=0&&slot<6)Model.TakeStored(view.StoragePage*6+slot);view.StoragePage=Math.Min(view.StoragePage,Math.Max(0,(Model.StoredItems.Count-1)/6));}
  else if(Screen=="storage"&&id=="deposit"){Model.DepositStored(view.Selected);view.Selected=Math.Max(0,Math.Min(view.Selected,Model.S.Inventory.Count-1));}
  else if(Screen=="storage"&&(id=="storage-prev"||id=="storage-next")){view.StoragePage=Math.Max(0,Math.Min(Math.Max(0,(Model.StoredItems.Count-1)/6),view.StoragePage+(id=="storage-next"?1:-1)));}
  else if(Screen=="storage"&&(id=="storage-slot-prev"||id=="storage-slot-next")){int count=Model.S.Inventory.Count;view.Selected=count==0?0:(Math.Max(0,view.Selected)+(id=="storage-slot-next"?1:count-1))%count;}
  else if(id=="talk"){if(Model.BeginTalk()){Screen="talk";returnScreen="";view.Selected=0;held.Clear();}}
  else if(Screen=="talk"&&id=="accept")Model.AcceptFavor();
  else if(Screen=="talk"&&id=="deliver")Model.DeliverFavor();
  else if(Screen=="talk"&&id.StartsWith("buy")){int item;if(int.TryParse(id.Substring(3),out item))Model.Buy(item);}
  else if(Screen=="talk"&&id=="sell"){Model.Sell(view.Selected);view.Selected=Math.Max(0,Math.Min(view.Selected,Model.S.Inventory.Count-1));}
  else if(Screen=="talk"&&(id=="sale-prev"||id=="sale-next")){int count=Model.S.Inventory.Count;view.Selected=count==0?0:(view.Selected+(id=="sale-next"?1:count-1))%count;}
  else if(Screen=="journal"&&id=="abandon")Model.AbandonFavor();
  else if(id=="restart"){Model.Reset();Screen="";held.Clear();Save();}
  else if(id=="pause"||id=="craft"||id=="journal"||id=="guide"||id=="confirm"){returnScreen=Screen=="intro"?"intro":Screen=="pause"?"pause":"";Screen=id;held.Clear();}
  else if(id=="make"&&Screen=="craft")Model.Craft();else if(id=="make-shovel"&&Screen=="craft")Model.CraftShovel();else if(id=="save"){if(Save())Model.Tell("Partidă salvată.");}else if(id=="quit")Close();Invalidate();}
 protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);using(var g=Graphics.FromImage(frame))view.Render(g,Model,Screen,hasSave,saveFailed);e.Graphics.InterpolationMode=InterpolationMode.HighQualityBilinear;e.Graphics.DrawImage(frame,ViewRect());Frames++;}
 protected override void Dispose(bool disposing){if(disposing){timer.Dispose();view.Dispose();frame.Dispose();}base.Dispose(disposing);}
}
static class Program {
 [STAThread] static int Main(string[] args){
  try{
   if(args.Length>=2&&args[0]=="--render"){Directory.CreateDirectory(args[1]);var m=new GameModel();using(var v=new GameView())using(var b=new Bitmap(GameView.Width,GameView.Height))using(var g=Graphics.FromImage(b)){foreach(string screen in new[]{"intro","","craft","guide","pause"}){v.Render(g,m,screen,false);b.Save(Path.Combine(args[1],screen==""?"game.png":screen+".png"),System.Drawing.Imaging.ImageFormat.Png);}}return 0;}
   Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);
   if(args.Length>=1&&args[0]=="--smoke-test"){using(var w=new GameWindow(true)){Application.Run(w);if(args.Length>=2)File.WriteAllText(args[1],"Window loop completed; ticks >= 30; rendered frames: "+w.Frames+"; clock progresses during play: "+w.ResumeVerified+"; clock frozen during pause: "+w.PauseVerified+"; dialogue freezes clock: "+w.DialoguePauseVerified+"; favor command flow: "+w.SocialCommandsVerified+"; shovel menu command: "+w.ShovelCommandVerified+"; storage transfers: "+w.StorageCommandVerified+"; storage pauses clock: "+w.StoragePauseVerified+"; accelerated wait command: "+w.WaitingCommandVerified+"; pause cancels waiting: "+w.WaitingPauseVerified);return w.Frames>0&&w.ResumeVerified&&w.PauseVerified&&w.DialoguePauseVerified&&w.SocialCommandsVerified&&w.ShovelCommandVerified&&w.StorageCommandVerified&&w.StoragePauseVerified&&w.WaitingCommandVerified&&w.WaitingPauseVerified?0:2;}}
   Application.Run(new GameWindow());return 0;
  }catch(Exception ex){if(args.Length>0){File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"error.txt"),ex.ToString());return 1;}MessageBox.Show("Jocul nu a putut porni.\n"+ex.Message,"Blocul 17",MessageBoxButtons.OK,MessageBoxIcon.Error);return 1;}
 }
}
}
