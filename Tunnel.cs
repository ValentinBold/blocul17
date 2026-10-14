using System;
using System.Linq;

namespace Blocul17 {
public partial class GameModel {
 public bool AtTunnel(){return Dist(S.X,S.Y,10.5,21.5)<1.2&&LOS(S.X,S.Y,10.5,21.5);}
 public bool CraftShovel(){
  if(S.Won||S.Health<=0||Action!="")return false;
  if(S.Intellect<40){Tell("Lopata cere 40 intelect. Citește de două ori în bibliotecă.");return false;}
  string[] need={"pilă","sârmă","bandă"};
  if(need.Any(i=>!S.Inventory.Contains(i))){Tell("Lopata cere pilă + sârmă + bandă.");return false;}
  foreach(string i in need)S.Inventory.Remove(i);S.Inventory.Add("lopată");Tell("Lopată construită. Caută trapa din colțul sud-estic al bibliotecii.");return true;
 }
 public bool UseTunnel(){
  if(!AtTunnel()||Action!=""||S.Won||S.Health<=0)return false;
  if(S.TunnelStage==3){
   if(Routine!="STINGEREA"){Tell("Tunelul este gata. Ieșirea este sigură numai între 22:00 și 07:00.");return false;}
   if(S.Heat>=60||Guards.Any(a=>a.Alert>0)){Tell("Pierde urmărirea înainte să intri în tunel.");return false;}
   S.X=.5;S.Y=21.5;S.Won=true;S.EscapedTunnel=true;Tell("Ai ieșit prin tunel, dincolo de ziduri. Ești liber.");return true;
  }
  if(!S.Inventory.Contains("lopată")){Tell("Trapa ascunde pământ moale. Construiește o lopată cu C: 40 intelect.");return false;}
  if(S.Energy<20){Tell("O etapă de săpat cere 20 energie.");return false;}
  Begin("Săparea tunelului",8);Tell("Sapi etapa "+(S.TunnelStage+1)+"/3. Mișcarea anulează doar etapa curentă.");return true;
 }
 void FinishDig(){S.TunnelStage++;S.Energy=Math.Max(0,S.Energy-20);Tell(S.TunnelStage==3?"Tunel terminat. Revino noaptea și apasă E la trapă.":"Tunel: "+S.TunnelStage+"/3 etape. Progresul terminat se păstrează.");}
}
}
