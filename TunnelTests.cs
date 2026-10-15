using System;
using System.IO;
using System.Linq;

namespace Blocul17 {
public static partial class Tests {
 static void TunnelChecks(){
  var m=new GameModel();m.S.Inventory.AddRange(new[]{"pilă","sârmă","bandă"});m.S.Intellect=39;
  Check(!m.CraftShovel()&&m.S.Inventory.Count==3,"shovel intellect gate preserves materials");m.S.Intellect=40;Check(m.CraftShovel()&&m.S.Inventory.SequenceEqual(new[]{"lopată"}),"shovel consumes recipe exactly once");Check(!m.CraftShovel(),"shovel cannot duplicate missing materials");
  Check(!m.UseTunnel(),"tunnel refuses remote interaction");m.S.X=10.5;m.S.Y=21.5;m.S.Minutes=730;m.S.Energy=19;Check(!m.UseTunnel(),"dig needs twenty energy");m.S.Energy=100;m.S.Inventory.Clear();Check(!m.UseTunnel(),"dig requires shovel");m.S.Inventory.Add("lopată");
  Check(m.UseTunnel(),"dig begins at library hatch");Wait(m,2);m.Tick(.02,1,0,false);Check(m.S.TunnelStage==0&&m.S.Energy==100&&m.Action=="","movement cancels current dig without charge");
  m.UseTunnel();Wait(m,8.02);Check(m.S.TunnelStage==1&&m.S.Energy>=80&&m.S.Energy<81,"completed dig advances one stage and charges twenty");Wait(m,1);Check(m.S.TunnelStage==1,"idle cannot duplicate tunnel progress");
  m.UseTunnel();Wait(m,1);string save=Path.Combine(output,"tunnel-save.xml");m.Save(save);var loaded=new GameModel();Check(loaded.Load(save)&&loaded.S.TunnelStage==1&&loaded.Action==""&&loaded.S.Inventory.Contains("lopată"),"save keeps completed stages and tool but not unfinished action");
  loaded.UseTunnel();Wait(loaded,8.02);loaded.UseTunnel();Wait(loaded,8.02);Check(loaded.S.TunnelStage==3&&!loaded.S.FenceOpen,"three digs finish tunnel without opening fence");Check(!loaded.UseTunnel()&&!loaded.S.Won,"daytime tunnel exit blocked");
  loaded.S.Minutes=1320;loaded.S.Heat=60;Check(!loaded.UseTunnel(),"high suspicion blocks tunnel exit");loaded.S.Heat=0;loaded.Guards[0].Alert=1;Check(!loaded.UseTunnel(),"active chase blocks tunnel exit");loaded.Guards[0].Alert=0;Check(loaded.UseTunnel()&&loaded.S.Won&&loaded.S.EscapedTunnel&&loaded.S.X<1,"night exit wins outside perimeter");loaded.Save(save);var win=new GameModel();Check(win.Load(save)&&win.S.EscapedTunnel&&win.S.Won,"tunnel victory round trips");
  double time=win.S.Minutes;Wait(win,1);Check(win.S.Minutes==time&&!win.UseTunnel()&&!win.CraftShovel(),"victory freezes simulation and actions");
  m.S.TunnelStage=4;m.Save(save);Check(!win.Load(save)&&win.S.Won,"invalid tunnel stage rejected without state replacement");m.S.TunnelStage=-1;m.Save(save);Check(!win.Load(save),"negative tunnel stage rejected");m.S.TunnelStage=1;m.S.EscapedTunnel=true;m.Save(save);Check(!win.Load(save),"inconsistent tunnel victory rejected");
  m.S.EscapedTunnel=false;m.S.TunnelStage=2;m.S.Money=21;m.Caught();Check(m.S.TunnelStage==0&&m.S.Money==21&&!m.S.Inventory.Contains("lopată"),"capture fills tunnel confiscates tool and preserves money");m.S.Health=0;Check(!m.UseTunnel()&&!m.CraftShovel(),"defeat blocks tunnel and crafting");m.Reset();Check(m.S.TunnelStage==0&&!m.S.EscapedTunnel,"new game resets tunnel");
  string fixture=Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"..","fixtures","save-v2.xml"));Check(m.Load(fixture)&&m.S.Version==5&&m.S.Money==27&&m.S.ActiveFavor==0&&m.S.WorkDone==2&&m.S.TunnelStage==0,"genuine v2 save migrates economy and initializes tunnel");
  m.Reset();m.S.X=10.5;m.S.Y=21.5;m.S.Minutes=730;m.S.Inventory.Add("lopată");m.Guards[0].X=10.5;m.Guards[0].Y=20.5;m.Guards[0].DX=0;m.Guards[0].DY=1;m.S.Heat=59;m.UseTunnel();Wait(m,7);Check(m.S.Caught>0&&m.S.TunnelStage==0,"guard catches witnessed digging");
  m.Reset();m.S.X=10.5;m.S.Y=21.5;m.S.TunnelStage=3;m.S.Minutes=420;Check(!m.UseTunnel(),"07:00 closes tunnel exit");m.S.Minutes=0;Check(m.UseTunnel(),"midnight permits completed tunnel exit");
  m.Reset();m.S.X=10.5;m.S.Y=21.5;m.S.Minutes=730;m.S.Inventory.Add("lopată");m.Guards[0].X=10.5;m.Guards[0].Y=20.5;m.Guards[0].DX=0;m.Guards[0].DY=1;m.UseTunnel();m.Tick(.02,0,0,false);Check(m.S.Heat>0,"witnessed digging raises suspicion from zero");
  // Complete third route using real movement, routines, loot, study and time.
  m.Reset();Walk(m,5.5,5.5);Loot(m);Loot(m);Walk(m,17.5,12.5);while(m.S.Minutes<480)m.Tick(.02,0,0,false);
  Walk(m,26.5,19.5);while(m.S.Minutes<540)m.Tick(.02,0,0,false);Walk(m,25.5,5.5);Loot(m);Loot(m);Walk(m,6.5,19.5);m.Interact();Wait(m,3.1);m.Interact();Wait(m,3.1);Shot(m,"tools","craft");Check(m.CraftShovel(),"natural campaign crafts shovel from found materials and study");
  Walk(m,10.5,21.5);for(int i=0;i<3;i++){m.Interact();Wait(m,8.1);}Check(m.S.TunnelStage==3&&m.S.Caught==0,"natural campaign completes three digs");Shot(m,"tunnel","");
  while(m.S.Minutes<1060)m.Tick(.02,0,0,false);Walk(m,17.5,12.5);while(m.S.Minutes<1140)m.Tick(.02,0,0,false);Walk(m,10.5,21.5);while(m.S.Minutes<1320)m.Tick(.02,0,0,false);m.Interact();
  Check(m.S.Won&&m.S.EscapedTunnel&&m.S.Caught==0&&!m.S.FenceOpen,"full tunnel campaign escapes at night with active guards and no captures");Shot(m,"tunnel-victory","");Console.WriteLine("Tunnel campaign: day "+m.S.Day+" "+m.Clock+"; captures "+m.S.Caught+"; fence remains intact.");
 }
}
}

