using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Blocul17 {
public static partial class Tests {
 static int count;static string output;
 static void Check(bool condition,string name){if(!condition)throw new Exception("FAIL: "+name);count++;Console.WriteLine("PASS: "+name);}
 static void Wait(GameModel m,double seconds){for(int i=0;i<(int)(seconds*50);i++)m.Tick(.02,0,0,false);}
 static void Walk(GameModel m,double tx,double ty){
  int startCaught=m.S.Caught;
  for(int i=0;i<5000;i++){
   if(GameModel.Dist(m.S.X,m.S.Y,tx,ty)<.13)return;
   int[] n=m.NextTile(m.S.X,m.S.Y,tx,ty);if(n==null)throw new Exception("Unreachable target "+tx+","+ty);
   double gx=n[0]+.5,gy=n[1]+.5;
   if((int)m.S.X==(int)tx&&(int)m.S.Y==(int)ty){gx=tx;gy=ty;}
   if(n[0]!=(int)m.S.X&&Math.Abs(m.S.Y-((int)m.S.Y+.5))>.06){gx=(int)m.S.X+.5;gy=(int)m.S.Y+.5;}
   if(n[1]!=(int)m.S.Y&&Math.Abs(m.S.X-((int)m.S.X+.5))>.06){gx=(int)m.S.X+.5;gy=(int)m.S.Y+.5;}
   m.Tick(.02,gx-m.S.X,gy-m.S.Y,false);
   if(m.S.Caught!=startCaught)throw new Exception("Caught while walking");
   if(m.S.Won)return;
  }throw new Exception("Movement stalled at "+m.S.X+","+m.S.Y+" target "+tx+","+ty);
 }
 static void Loot(GameModel m){m.Interact();if(m.StorageCrate<0||!m.TakeStored(0))throw new Exception("Could not take container item");m.StorageCrate=-1;}
 static void Shot(GameModel m,string name,string screen){using(var v=new GameView())using(var b=new Bitmap(1280,900))using(var g=Graphics.FromImage(b)){v.Render(g,m,screen,false);b.Save(Path.Combine(output,name+".png"),System.Drawing.Imaging.ImageFormat.Png);}}
 static GameModel Campaign(double delay){
  var m=new GameModel();Walk(m,5.5,5.5);Loot(m);Loot(m);Walk(m,17.5,12.5);
  while(m.S.Minutes<480)m.Tick(.02,0,0,false);
  Walk(m,26.5,19.5);while(m.S.Minutes<540)m.Tick(.02,0,0,false);
  Walk(m,25.5,5.5);Loot(m);Loot(m);Walk(m,6.5,19.5);m.Interact();Wait(m,3.1);
  if(!m.Craft())throw new Exception("Could not craft naturally");
  Walk(m,31.5,7.5);Wait(m,delay);Walk(m,31.5,12.5);m.Interact();Wait(m,8.1);
  if(m.S.Caught>0||!m.S.FenceOpen)return null;
  Shot(m,"escape-in-progress","");Walk(m,33.5,12.5);return m;
 }
 static GameModel EconomicCampaign(double delay){
  var m=new GameModel();Walk(m,5.5,5.5);Loot(m);Loot(m);Walk(m,15.5,12.5);
  if(!m.BeginTalk()||m.TalkNpc!=0||!m.AcceptFavor())throw new Exception("Natural favor could not start");Shot(m,"favor","talk");if(!m.DeliverFavor())throw new Exception("Natural favor failed");m.TalkNpc=-1;
  while(m.S.Minutes<480)m.Tick(.02,0,0,false);Walk(m,26.5,19.5);while(m.S.Minutes<540)m.Tick(.02,0,0,false);
  Walk(m,24.5,6.5);m.Interact();Wait(m,6.1);if(m.S.Money!=30||m.S.WorkDone!=1)throw new Exception("Natural work not paid");
  Walk(m,25.5,6.5);if(!m.BeginTalk()||m.TalkNpc!=1)throw new Exception("Natural trader unreachable");Shot(m,"trade","talk");
  if(!m.Buy(1)||!m.Buy(2)||m.S.Money!=8)throw new Exception("Natural trade failed");m.TalkNpc=-1;
  Walk(m,6.5,19.5);m.Interact();Wait(m,3.1);if(!m.Craft())throw new Exception("Economic crafting failed");
  Walk(m,31.5,7.5);Wait(m,delay);Walk(m,31.5,12.5);m.Interact();Wait(m,8.1);if(m.S.Caught>0||!m.S.FenceOpen)return null;
  Walk(m,33.5,12.5);return m;
 }
 static void Near(GameModel m,int npc){m.S.X=m.Inmates[npc].X;m.S.Y=m.Inmates[npc].Y;m.TalkNpc=npc;}
 static void EconomyChecks(){
  var m=new GameModel();m.S.X=24.5;m.S.Y=6.5;
  Check(!m.StartWork(),"work closed outside 09-12");m.S.Minutes=550;m.S.Energy=14;Check(!m.StartWork(),"work needs fifteen energy");m.S.Energy=100;Check(m.StartWork(),"work starts at actual bench");Wait(m,2);m.Tick(.02,1,0,false);Check(m.S.Money==0&&m.S.WorkDone==0&&m.Action=="","interrupted work has no payment");
  m.S.X=24.5;m.S.Y=6.5;Check(m.StartWork(),"work can restart");Wait(m,6.02);Check(m.S.Money==12&&m.S.WorkDone==1&&m.S.Energy>=84&&m.S.Energy<86,"completed piece paid once and energy charged");Wait(m,1);Check(m.S.Money==12,"completed work cannot pay again on idle ticks");
  m.StartWork();Wait(m,6.02);m.StartWork();Wait(m,6.02);Check(m.S.WorkDone==3&&m.S.Money==36&&!m.StartWork(),"daily work quota enforced");
  m.Reset();m.S.X=24.5;m.S.Y=6.5;m.S.Minutes=718;m.StartWork();Wait(m,2);Check(m.Action==""&&m.S.Money==0&&m.S.WorkDone==0,"noon cancels incomplete work without pay");
  m.Reset();Near(m,0);Check(m.BeginTalk()&&m.TalkNpc==0&&m.AcceptFavor(),"favor accepted with nearby prisoner");Check(!m.DeliverFavor()&&m.S.Money==0,"missing favor item cannot pay");m.S.Inventory.Add("biscuiți");Check(m.DeliverFavor()&&m.S.Money==18&&!m.S.Inventory.Contains("biscuiți")&&m.S.ActiveFavor==-1,"favor consumes item and pays once");Check(!m.DeliverFavor()&&!m.AcceptFavor()&&m.S.Money==18,"favor cannot be claimed twice on one day");
  Near(m,2);Check(m.AcceptFavor(),"different favor available");Near(m,3);Check(!m.AcceptFavor()&&m.S.ActiveFavor==1,"one active favor at a time");m.S.Inventory.Add("bandă");Check(!m.DeliverFavor()&&m.S.Inventory.Contains("bandă"),"wrong prisoner cannot consume quest item");m.AbandonFavor();Check(m.S.ActiveFavor==-1&&m.S.Inventory.Contains("bandă"),"abandoning favor preserves items");
  Near(m,2);m.AcceptFavor();m.Caught();Check(m.S.ActiveFavor==1&&m.S.Money==18,"capture preserves money and accepted favor");
  m.Reset();Near(m,1);m.S.Money=5;Check(!m.Buy(0)&&m.S.Money==5&&m.S.TradeStock[0]==2,"insufficient cash leaves transaction unchanged");m.S.Money=100;for(int i=0;i<6;i++)m.S.Inventory.Add("bandă");Check(!m.Buy(1)&&m.S.Money==100&&m.S.TradeStock[1]==2,"full inventory does not spend money or stock");m.S.Inventory.Clear();Check(m.Buy(1)&&m.S.Money==88&&m.S.TradeStock[1]==1&&m.S.Inventory[0]=="pilă","purchase consumes money and stock atomically");
  Check(m.Buy(1)&&!m.Buy(1)&&m.S.Money==76&&m.S.TradeStock[1]==0,"sold out item cannot be bought");Check(m.Sell(0)&&m.S.Money==82&&m.S.TradeStock[1]==1&&m.S.Inventory.Count==1,"sale transfers one item to shop and pays resale price");m.S.Inventory.Add("clește");Check(!m.Sell(1)&&m.S.Inventory.Contains("clește"),"cutter cannot be sold accidentally");
  m.S.X=5.5;m.S.Y=6.5;Check(!m.Buy(0)&&!m.Sell(0),"trade refused remotely");Near(m,1);m.S.Heat=60;Check(!m.BeginTalk()&&!m.Buy(0),"suspicion blocks negotiation");m.S.Heat=0;m.Guards[0].Alert=4;Check(!m.Buy(0),"active chase blocks trade");
  m.Reset();m.Inmates[0].X=9.1;m.Inmates[0].Y=5.5;m.S.X=7.5;m.S.Y=5.5;Check(!m.CanTalkTo(0),"cannot talk through wall despite proximity");
  m.Reset();m.S.WorkDone=3;m.S.TradeStock=new[]{0,0,0,0};m.S.FavorPaidDay[0]=1;m.S.Minutes=1439.98;Wait(m,.04);Near(m,0);Check(m.S.Day==2&&m.S.WorkDone==0&&m.S.TradeStock.All(n=>n==2)&&m.AcceptFavor(),"new day replenishes work shop and completed favor");
  string save=Path.Combine(output,"economy-save.xml");m.S.Money=27;m.S.WorkDone=2;m.S.TradeStock[1]=0;m.Save(save);var restored=new GameModel();Check(restored.Load(save)&&restored.S.Version==5&&restored.S.Money==27&&restored.S.WorkDone==2&&restored.S.TradeStock[1]==0&&restored.S.ActiveFavor==0,"economy and pending favor survive save load");
  m.S.Money=-1;m.Save(save);Check(!restored.Load(save)&&restored.S.Money==27,"negative balance rejected without replacing state");m.S.Money=1;m.S.TradeStock=new[]{2};m.Save(save);Check(!restored.Load(save),"invalid shop array rejected");m.S.TradeStock=new[]{2,2,2,2};m.S.WorkDone=4;m.Save(save);Check(!restored.Load(save),"invalid daily quota rejected");
  string legacy=Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"..","fixtures","save-v1.xml"));Check(File.Exists(legacy)&&restored.Load(legacy)&&restored.S.Version==5&&restored.S.Intellect==40&&restored.S.Inventory.Contains("bandă")&&restored.S.Money==0&&restored.S.ActiveFavor==-1&&restored.S.TradeStock.All(n=>n==2),"real v0.1 serialized save migrates with preserved progress");
  restored.Save(save);var again=new GameModel();Check(again.Load(save)&&again.S.Version==5&&again.S.Intellect==40,"migrated save round trips in v5");
  m.Reset();m.S.X=5.5;m.S.Y=5.5;m.S.Crates[0].Items=Enumerable.Repeat("bandă",100).ToList();m.S.Inventory.Add("pilă");m.Drop(0);Check(m.S.Inventory.Contains("pilă")&&m.S.Crates[0].Items.Count==100,"full desk preserves item and valid save capacity");m.Save(save);Check(again.Load(save),"maximum legitimate desk capacity loads successfully");
 }
 public static int Main(string[] args){try{
  output=args.Length>0?Path.GetFullPath(args[0]):Path.GetFullPath("qa");Directory.CreateDirectory(output);
  var m=new GameModel();Check(m.Free(m.S.X,m.S.Y),"valid starting position");
  foreach(var p in new[]{new[]{17,12},new[]{25,5},new[]{6,19},new[]{26,19},new[]{31,12}})Check(m.NextTile(m.S.X,m.S.Y,p[0]+.5,p[1]+.5)!=null,"reachable essential point "+p[0]+","+p[1]);
  m.S.X=4.5;m.S.Y=4.5;for(int i=0;i<120;i++)m.Tick(.02,-1,0,false);Check(m.S.X>=4.23,"wall collision stops player");
  Check(!m.LOS(5.5,5.5,9.5,5.5),"walls block guard vision");Check(m.LOS(14.5,12.5,20.5,12.5),"clear yard line of sight");
  m.Reset();for(int i=0;i<6;i++)Check(m.Add("bandă"),"inventory accepts slot "+(i+1));Check(!m.Add("pilă")&&m.S.Inventory.Count==6,"inventory rejects overflow without loss");
  m.Reset();m.S.Inventory.AddRange(new[]{"pilă","sârmă","bandă"});Check(!m.Craft()&&m.S.Inventory.Count==3,"insufficient intellect does not consume materials");m.S.Intellect=30;Check(m.Craft()&&m.S.Inventory.SequenceEqual(new[]{"clește"}),"craft consumes recipe and creates one cutter");Check(!m.Craft()&&m.S.Inventory.Count==1,"cannot duplicate crafted item");
  m.S.X=5.5;m.S.Y=5.5;m.Drop(0);Check(m.S.Inventory.Count==0&&m.S.Crates[0].Items.Contains("clește"),"return to desk preserves item");
  m.Reset();m.S.X=6.5;m.S.Y=19.5;m.Interact();Wait(m,1);m.Tick(.02,1,0,false);Check(m.Action==""&&m.S.Intellect==20,"movement cancels study without reward");m.Interact();Wait(m,3.2);Check(m.S.Intellect==30,"completed study grants exactly ten intellect");
  m.Reset();m.S.X=31.5;m.S.Y=12.5;m.Interact();Check(m.Action==""&&!m.S.FenceOpen,"cannot cut without tool");m.S.Inventory.Add("clește");m.S.Energy=29;m.Interact();Check(m.Action=="","cannot cut without energy");m.S.Energy=100;m.Interact();m.Tick(.02,0,-1,false);Check(!m.S.FenceOpen&&m.S.Energy>=99,"interrupted cutting does not open fence");
  m.Reset();m.S.Inventory.Add("clește");m.S.Crates[1].Items.Clear();m.Caught();Check(m.S.Inventory.Count==0&&m.S.Health==75&&m.S.Crates[1].Items.Contains("sârmă"),"capture confiscates and restores essential loot");
  m.S.X=5.5;m.S.Y=7.5;m.Interact();Wait(m,4.2);Check(m.S.Health==100,"bed restores health after capture");
  m.Reset();m.S.Minutes=445;Wait(m,2);Check(m.S.Heat>0,"missed roll call raises suspicion after grace");m.S.X=17.5;m.S.Y=12.5;double heat=m.S.Heat;Wait(m,1);Check(m.S.Heat<heat,"attendance reduces suspicion");
  m.Reset();m.S.X=31.5;m.S.Y=12.5;m.S.Inventory.Add("clește");m.Guards[0].X=24.5;m.Guards[0].Y=13.5;m.Guards[0].Target=2;m.Guards[0].DX=1;m.Guards[0].DY=0;m.Interact();Wait(m,6);Check(m.S.Caught>0,"guard catches witnessed fence cutting");
  m.Reset();m.S.Inventory.Add("bandă");m.S.Intellect=40;Wait(m,2);double guardX=m.Guards[0].X,guardY=m.Guards[0].Y;string save=Path.Combine(output,"test-save.xml");m.Save(save);var loaded=new GameModel();Check(loaded.Load(save)&&loaded.S.Intellect==40&&loaded.S.Inventory.Contains("bandă"),"save and load preserve progress");Check(loaded.Guards[0].X==guardX&&loaded.Guards[0].Y==guardY,"save and load preserve patrol position");
  File.WriteAllText(save,"not xml");double before=loaded.S.Intellect;Check(!loaded.Load(save)&&loaded.S.Intellect==before,"corrupt save rejected without changing state");m.S.X=double.NaN;m.Save(save);Check(!loaded.Load(save),"NaN in save is rejected");m.Reset();m.S.Crates[0].X=999;m.Save(save);Check(!loaded.Load(save),"invalid crate location rejected");m.Reset();m.S.X=3.5;m.S.Y=3.5;m.Save(save);Check(!loaded.Load(save)&&loaded.Free(loaded.S.X,loaded.S.Y),"wall spawn rejected and old state retained");
  m.Reset();m.S.Minutes=1439.98;Wait(m,.04);Check(m.S.Day==2&&m.S.Minutes<1,"midnight rollover");
  var patrol=new GameModel();double gx=patrol.Guards[0].X,gy=patrol.Guards[0].Y;Wait(patrol,30);Check(GameModel.Dist(gx,gy,patrol.Guards[0].X,patrol.Guards[0].Y)>3,"guards navigate patrol over time");Check(patrol.Guards.All(a=>patrol.Free(a.X,a.Y)),"guards remain outside walls");
  GameModel win=null;double winningDelay=-1;for(int delay=0;delay<=90;delay+=3){try{var trial=Campaign(delay);if(trial!=null&&trial.S.Won){win=trial;winningDelay=delay;break;}}catch(Exception ex){if(ex.Message.Contains("Unreachable")||ex.Message.Contains("stalled"))throw;}}
  Check(win!=null,"full new-game escape using movement, interactions, normal clock and active guards");Check(win.S.Caught==0,"successful campaign has no captures");Shot(win,"victory","");Console.WriteLine("Campaign: hide for "+winningDelay+" seconds before fence approach; escape day "+win.S.Day+" "+win.Clock+".");
  EconomyChecks();GameModel economicWin=null;int economicDelay=-1;for(int delay=0;delay<=90;delay+=3){try{var result=EconomicCampaign(delay);if(result!=null&&result.S.Won){economicWin=result;economicDelay=delay;break;}}catch(Exception ex){if(ex.Message!="Caught while walking")throw;}}
  Check(economicWin!=null&&economicWin.S.Caught==0,"new game to escape via favor work purchase and crafting with live guards");Check(economicWin.S.Money==8&&economicWin.S.WorkDone==1&&economicWin.S.FavorPaidDay[0]==1,"economic campaign balances are exact");Shot(economicWin,"economic-victory","");Console.WriteLine("Economic campaign: delay "+economicDelay+" s; escaped "+economicWin.Clock+"; remaining cash "+economicWin.S.Money+".");
  TunnelChecks();StorageChecks();WaitingChecks();SecurityChecks();
  var perf=new GameModel();var sw=Stopwatch.StartNew();using(var v=new GameView())using(var b=new Bitmap(1280,900))using(var g=Graphics.FromImage(b)){for(int i=0;i<100;i++){perf.Tick(.02,0,0,false);v.Render(g,perf,"",false);}}sw.Stop();Console.WriteLine("Combined simulation + offscreen rendering mean: "+(sw.Elapsed.TotalMilliseconds/100).ToString("F2")+" ms/frame (not display FPS).");
  Console.WriteLine("TOTAL PASS: "+count);return 0;
 }catch(Exception ex){Console.WriteLine(ex.ToString());return 1;}}
}
}
