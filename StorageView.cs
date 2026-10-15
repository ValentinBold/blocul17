using System;

namespace Blocul17 {
public partial class GameView {
 public int StoragePage=0;
 void DrawStorage(GameModel m,int x,int y){
  var items=m.StoredItems;int pages=Math.Max(1,(items.Count+5)/6);StoragePage=Math.Max(0,Math.Min(StoragePage,pages-1));
  Text(m.HiddenStorage?"Ascunzătoarea ta":m.S.Crates[m.StorageCrate].Name,x+35,y+68,30,Ink,true);
  Text((m.HiddenStorage?"Protejată la control și capturare · "+items.Count+"/3":items.Count+" obiecte · conținut neprotejat")+" · timpul este oprit",x+38,y+111,14,m.HiddenStorage?Green:Muted);
  Button("storage-toggle",m.HiddenStorage?"H · Vezi biroul":"H · Ascunzătoare",x+38,y+143,248,33,false,m.StorageCrate==0);
  Button("storage-prev","‹",x+302,y+143,36,33,false,StoragePage>0);Center((StoragePage+1)+" / "+pages,x+346,y+148,158,14,Muted);Button("storage-next","›",x+514,y+143,48,33,false,StoragePage<pages-1);
  for(int i=0;i<6;i++){int row=y+190+i*35,index=StoragePage*6+i;Box(Dark,x+38,row,524,31);if(index<items.Count){Text((i+1)+"  "+items[index]+(GameModel.IsContraband(items[index])?"  !":""),x+50,row+5,16,Ink);Button("take"+i,"Ia",x+460,row+2,90,27,true,m.S.Inventory.Count<6);}else Text("—",x+50,row+5,16,Muted);}
  Text("BUZUNAR · ← / → SELECTEAZĂ · "+m.S.Inventory.Count+"/6",x+38,y+410,12,Muted,true);
  int slot=Math.Max(0,Selected);string item=slot<m.S.Inventory.Count?m.S.Inventory[slot]:"niciun obiect";
  Button("storage-slot-prev","‹",x+38,y+434,33,33);Button("storage-slot-next","›",x+79,y+434,33,33);Text(item,x+127,y+440,16,Ink);Button("deposit","V · Depozitează",x+333,y+434,217,33,false,m.StorageCrate==0&&slot<m.S.Inventory.Count&&items.Count<(m.HiddenStorage?3:100));
  Wrap(m.Message,x+38,y+476,516,37,13,Muted);Button("close","Închide · Esc",x+38,y+524,524,37);
 }
}
}
