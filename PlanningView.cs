using System;

namespace Blocul17 {
public partial class GameView {
 public int PlanTab=0;
 void DrawPlanning(GameModel m,int x,int y){
  Text("Planul de evadare",x+35,y+68,32,Ink,true);
  Text("Compară rutele · timpul este oprit",x+38,y+111,15,Muted);
  Button("plan-fence","1 · Gard",x+38,y+143,166,35,PlanTab==0);
  Button("plan-tunnel","2 · Tunel",x+217,y+143,166,35,PlanTab==1);
  Button("plan-schedule","3 · Program",x+396,y+143,166,35,PlanTab==2);
  if(PlanTab==2){
   for(int i=0;i<GameModel.ScheduleTimes.Length;i++){
    int row=y+193+i*31;Box(Dark,x+38,row,524,28);
    Text(GameModel.ScheduleClock(GameModel.ScheduleTimes[i]),x+48,row+5,13,Orange,true);
    Text(GameModel.ScheduleNames[i],x+160,row+5,14,Ink);
   }
   Text("Urmează: "+m.NextScheduleEvent,x+38,y+485,14,Green,true);
  }else{
   bool tunnel=PlanTab==1;Text(m.PlanProgress(tunnel),x+38,y+194,16,Orange,true);
   Text("OBIECT",x+48,y+228,11,Muted,true);string[] labels={"BUZUNAR","BIROU","ASCUNS","CONFISCAT"};for(int i=0;i<4;i++)Center(labels[i],x+206+i*88,y+228,84,10,Muted,true);
   for(int i=0;i<5;i++){int row=y+250+i*25;Box(Dark,x+38,row,524,23);Text(GameModel.PlanItems[i],x+48,row+2,14,Ink);int[] stocks=m.PlanStock(GameModel.PlanItems[i]);for(int j=0;j<4;j++)Center(stocks[j].ToString(),x+206+j*88,row+2,84,13,stocks[j]>0?Green:Muted);}
   Text("URMĂTORUL PAS · "+(tunnel?"40":"30")+" INTELECT PENTRU CRAFTING",x+38,y+391,11,Orange,true);
   Wrap(m.PlanAdvice(tunnel),x+38,y+417,524,73,18,Ink);
   Text("Numerele arată locul obiectelor; transferurile se fac lângă ele.",x+38,y+494,12,Muted);
  }
  Button("close","Înapoi · Esc",x+38,y+524,524,36);
 }
}
}
