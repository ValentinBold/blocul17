using System;
using System.Linq;

namespace Blocul17 {
public partial class GameModel {
 public bool Waiting=false;
 public bool CanWait(){return !S.Won&&S.Health>0&&Action==""&&AtRoutine()&&S.Heat<25&&!Guards.Any(g=>g.Alert>0);}
 public bool ToggleWait(){
  if(Waiting){StopWaiting("Așteptare oprită.");return false;}
  if(!CanWait()){Tell("Așteaptă după ce ajungi la program, termini acțiunea și pierzi urmărirea. Suspiciune sub 25.");return false;}
  Waiting=true;Tell("Aștepți ×20 până la schimbarea programului. T sau mișcarea oprește așteptarea.");return true;
 }
 public void StopWaiting(string message){if(!Waiting)return;Waiting=false;Tell(message);}
 // Run the same bounded simulation steps; never jump the clock or skip patrol updates.
 public void Advance(double dt,double mx,double my,bool sprint){
  if(mx!=0||my!=0)StopWaiting("Așteptare oprită prin mișcare.");
  if(!Waiting){Tick(dt,mx,my,sprint);return;}
  if(!CanWait()){StopWaiting("Așteptare oprită: atenție la program sau patrule.");return;}
  string routine=Routine;int day=S.Day;
  for(int i=0;i<20;i++){
   Tick(dt,0,0,false);
   if(!Waiting)return; // Capture resets transient state and supplies its own message.
   if(Routine!=routine||S.Day!=day){StopWaiting("Așteptare oprită: "+Routine.ToLower()+" · "+Clock+".");return;}
   if(!CanWait()){StopWaiting("Așteptare oprită: pericol în apropiere.");return;}
  }
 }
}
}
