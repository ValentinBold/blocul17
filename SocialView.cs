using System;

namespace Blocul17 {
public partial class GameView {
 void DrawSocial(GameModel m,int x,int y){
  int n=m.TalkNpc;Text(GameModel.NpcNames[n],x+35,y+69,35,Ink,true);Text(GameModel.NpcRoles[n]+" · timpul este oprit",x+38,y+114,15,Muted);Text(m.S.Money+" cr.",x+410,y+78,22,Orange,true);
  if(n==1){
   for(int i=0;i<4;i++){int ry=y+151+i*58;Box(Dark,x+38,ry,524,51);Item(GameModel.ShopItems[i],x+47,ry+13);Text(GameModel.ShopItems[i],x+100,ry+3,18,Ink,true);Text("Stoc: "+m.S.TradeStock[i]+" · "+GameModel.BuyPrices[i]+" cr.",x+101,ry+28,12,Muted);Button("buy"+i,(i+1)+"  Cumpără · "+GameModel.BuyPrices[i]+" cr.",x+333,ry+8,217,35,true,m.S.TradeStock[i]>0&&m.S.Money>=GameModel.BuyPrices[i]&&m.S.Inventory.Count<6);}
   int slot=Selected<0?0:Selected;string item=slot<m.S.Inventory.Count?m.S.Inventory[slot]:"niciun obiect";int price=m.SalePrice(slot);
   Text("OBIECT DIN BUZUNAR · ← / → SELECTEAZĂ",x+38,y+393,11,Muted,true);Button("sale-prev","‹",x+38,y+414,33,35);Button("sale-next","›",x+79,y+414,33,35);Text(item,x+126,y+420,17,Ink);Button("sell","Vinde · "+price+" cr.  [V]",x+333,y+414,217,35,false,price>0);
   Wrap(m.Message,x+38,y+459,516,43,14,Muted);Button("close","Încheie schimbul · Esc",x+38,y+516,524,40);
  }else{
   Wrap(m.Dialogue(n),x+38,y+162,510,98,23,Ink);
   Box(Dark,x+38,y+284,524,106);Text(n==4?"UN SFAT BUN NU COSTĂ":"O ÎNȚELEGERE SIMPLĂ",x+52,y+299,12,Orange,true);
   Wrap(n==4?"Bancul din stânga atelierului plătește 12 credite pentru o piesă. Radu are tot ce-ți trebuie pentru clește.":"Un singur favor activ. Adu obiectul în buzunare și revino la același deținut. Fiecare favor se plătește o dată pe zi.",x+52,y+327,491,59,16,Muted);
   Wrap(m.Message,x+38,y+412,515,71,16,Ink);
   int favor=m.FavorForNpc(n);if(favor>=0){bool deliver=m.S.ActiveFavor==favor;bool can=deliver?m.S.Inventory.Contains(GameModel.FavorItems[favor]):m.S.ActiveFavor<0&&m.S.FavorPaidDay[favor]!=m.S.Day;Button(deliver?"deliver":"accept",deliver?"Predă obiectul · Enter":"Acceptă favorul · Enter",x+38,y+507,331,44,true,can);Button("close","Înapoi · Esc",x+384,y+507,178,44);}else Button("close","Țin minte. · Esc",x+38,y+507,524,44,true);
  }
 }
}
}
