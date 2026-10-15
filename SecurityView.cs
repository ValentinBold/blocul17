using System;
using System.Linq;

namespace Blocul17 {
public partial class GameView {
 void DrawSecurity(GameModel m,int x,int y){
  Text("Securitate și contrabandă",x+35,y+70,30,Ink,true);
  Text("Planifică ce porți și ce lași în urmă.",x+38,y+113,16,Muted);
  Box(Dark,x+38,y+153,524,92);Text("01 / PERCHEZIȚIA BIROULUI",x+52,y+165,12,Orange,true);
  Text(m.InspectionStatus,x+52,y+187,19,Ink,true);
  Text("Anunț 18:00 · ascunzătoarea și buzunarele sunt protejate",x+52,y+218,14,Muted);
  Box(Dark,x+38,y+257,524,100);Text("02 / DETECTORUL ATELIERULUI",x+52,y+269,12,Orange,true);
  Text(m.MaterialPermit?"Permis materiale activ până la 12:00":"Materialele nu au permis în acest moment",x+52,y+292,18,m.MaterialPermit?Green:Ink,true);
  Wrap("Pilă și sârmă: permise 09:00–12:00. Clește și lopată: mereu interzise. O trecere detectată adaugă 35 suspiciune.",x+52,y+319,494,36,13,Muted);
  Box(Dark,x+38,y+369,524,83);Text("03 / OBIECTELE TALE",x+52,y+381,12,Orange,true);
  Text(m.PocketRisk+" interzise în buzunare · "+m.S.Stash.Count+"/3 ascunse",x+52,y+404,19,Ink,true);
  Text(m.S.Evidence.Sum()+" confiscate · ghișeul de lângă cantină, 12:00–22:00",x+52,y+431,14,Muted);
  Text("! marchează contrabanda. Capturarea golește buzunarele.",x+38,y+478,14,Muted);
  Button("close","Înapoi · Esc",x+38,y+518,524,40,true);
 }
 void DrawEvidence(GameModel m,int x,int y){
  Text("Depozitul de confiscări",x+35,y+70,30,Ink,true);
  Text("Recuperează câte un obiect · "+m.S.Money+" credite",x+38,y+114,17,Orange,true);
  Text("12:00–22:00 · fără urmărire · timpul este oprit",x+38,y+144,14,Muted);
  for(int i=0;i<4;i++){
   int row=y+186+i*66;Box(Dark,x+38,row,524,57);Item(GameModel.Contraband[i],x+45,row+13);
   Text((i+1)+" · "+GameModel.Contraband[i],x+96,row+7,18,Ink,true);Text(m.S.Evidence[i]+" în depozit",x+96,row+33,13,Muted);
   Button("recover"+i,GameModel.RecoveryPrices[i]+" cr. · Recuperează",x+300,row+11,248,35,true,m.CanRecover()&&m.S.Evidence[i]>0&&m.S.Money>=GameModel.RecoveryPrices[i]&&m.S.Inventory.Count<6);
  }
  Wrap(m.Message,x+38,y+459,524,49,14,Muted);Button("close","Închide · Esc",x+38,y+518,524,40);
 }
}
}
