using System;
using System.IO;
using System.Linq;

namespace Blocul17 {
public static partial class Tests {
 static void UntilRoutine(GameModel m){
  if(!m.ToggleWait())throw new Exception("Cannot start natural waiting");
  for(int i=0;i<3000&&m.Waiting;i++)m.Advance(.02,0,0,false);
  if(m.Waiting)throw new Exception("Waiting did not stop");
 }
 static void WaitingChecks(){
  var m=new GameModel();Check(!m.ToggleWait(),"cannot wait while absent from roll call");m.S.X=17.5;m.S.Y=12.5;Check(m.ToggleWait(),"can wait when attending routine");
  var baseline=new GameModel();baseline.S.X=17.5;baseline.S.Y=12.5;for(int i=0;i<20;i++)baseline.Tick(.02,0,0,false);m.Advance(.02,0,0,false);
  Check(m.S.Minutes==baseline.S.Minutes&&m.S.Energy==baseline.S.Energy&&m.S.Heat==baseline.S.Heat&&m.Guards.Zip(baseline.Guards,(a,b)=>a.X==b.X&&a.Y==b.Y&&a.Target==b.Target).All(v=>v)&&m.Inmates.Zip(baseline.Inmates,(a,b)=>a.X==b.X&&a.Y==b.Y).All(v=>v),"accelerated frame equals twenty ordinary patrol and prisoner simulation steps");
  Check(!m.ToggleWait()&&!m.Waiting,"T toggles waiting off");m.ToggleWait();double time=m.S.Minutes;m.Advance(.02,1,0,false);Check(!m.Waiting&&Math.Abs(m.S.Minutes-time-.04)<.00001,"movement cancels acceleration before a single normal step");
  m.S.Heat=25;Check(!m.ToggleWait(),"suspicion threshold blocks waiting");m.S.Heat=0;m.Guards[0].Alert=1;Check(!m.ToggleWait(),"chase blocks waiting");m.Guards[0].Alert=0;m.Action="Studiu";Check(!m.ToggleWait(),"ongoing work blocks waiting");m.Action="";m.S.Health=0;Check(!m.ToggleWait(),"defeat blocks waiting");m.S.Health=100;m.S.Won=true;Check(!m.ToggleWait(),"victory blocks waiting");
  m.Reset();m.S.X=17.5;m.S.Y=12.5;m.S.Minutes=479.98;m.ToggleWait();m.Advance(.02,0,0,false);Check(!m.Waiting&&m.Routine=="MIC DEJUN"&&m.S.Minutes<480.1,"waiting stops immediately at routine boundary");
  m.Reset();m.S.Minutes=1439.98;m.S.WorkDone=3;m.S.TradeStock=new[]{0,0,0,0};m.ToggleWait();m.Advance(.02,0,0,false);Check(!m.Waiting&&m.S.Day==2&&m.S.WorkDone==0&&m.S.TradeStock.All(n=>n==2),"midnight stops waiting and applies daily reset once");
  m.Reset();m.S.Minutes=1330;m.S.X=5.5;m.S.Y=5.5;m.ToggleWait();m.S.X=17.5;m.S.Y=12.5;time=m.S.Minutes;m.Advance(.02,0,0,false);Check(!m.Waiting&&m.S.Minutes==time,"unsafe state stops before fast simulation");
  m.Reset();m.S.Minutes=600;m.S.X=31.5;m.S.Y=12.5;m.S.Heat=24.9;m.Guards[0].X=30.5;m.Guards[0].Y=12.5;m.Guards[0].DX=1;m.Guards[0].DY=0;m.ToggleWait();m.Action="Tăierea gardului";m.Advance(.02,0,0,false);Check(!m.Waiting,"action introduced after waiting cancels acceleration");
  m.Reset();m.S.Minutes=730;m.ToggleWait();string save=Path.Combine(output,"wait-save.xml");m.Save(save);Check(m.Load(save)&&!m.Waiting,"loading never resumes accelerated time");m.ToggleWait();m.Caught();Check(!m.Waiting,"capture cancels accelerated time");m.S.Minutes=730;m.ToggleWait();m.Reset();Check(!m.Waiting,"new game cancels accelerated time");
  // Natural tunnel route with accelerated waits at every routine boundary.
  m.Reset();Walk(m,5.5,5.5);Loot(m);Loot(m);Walk(m,17.5,12.5);UntilRoutine(m);Check(m.Routine=="MIC DEJUN","accelerated campaign reaches breakfast without skipping it");Walk(m,26.5,19.5);UntilRoutine(m);Walk(m,25.5,5.5);Loot(m);Loot(m);Walk(m,6.5,19.5);m.Interact();Wait(m,3.1);m.Interact();Wait(m,3.1);m.CraftShovel();Walk(m,10.5,21.5);for(int i=0;i<3;i++){m.Interact();Wait(m,8.1);}
  m.ToggleWait();Shot(m,"waiting","");while(m.Waiting)m.Advance(.02,0,0,false);Check(m.Routine=="TIMP LIBER","wait stops at end of workshop");UntilRoutine(m);Check(m.Routine=="APEL"&&m.S.Minutes<1080.1,"wait stops at evening roll call before absence penalty");Walk(m,17.5,12.5);UntilRoutine(m);Walk(m,10.5,21.5);UntilRoutine(m);m.Interact();Check(m.S.Won&&m.S.EscapedTunnel&&m.S.Caught==0,"full accelerated-wait tunnel campaign wins without captures or clock jumps");Console.WriteLine("Accelerated campaign: day "+m.S.Day+" "+m.Clock+"; captures "+m.S.Caught+".");
 }
}
}
