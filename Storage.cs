using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocul17 {
public partial class GameModel {
 public int StorageCrate=-1;
 public bool HiddenStorage=false;
 public bool CanOpenStorage(int index){
  if(index<0||index>=S.Crates.Count||S.Won||S.Health<=0||Action!=""||S.Heat>=60||Guards.Any(a=>a.Alert>0))return false;
  Crate c=S.Crates[index];return Dist(S.X,S.Y,c.X+.5,c.Y+.5)<1.65&&LOS(S.X,S.Y,c.X+.5,c.Y+.5);
 }
 public bool BeginStorage(){
  Crate c=NearbyCrate();int index=c==null?-1:S.Crates.IndexOf(c);
  if(!CanOpenStorage(index)){Tell("Apropie-te de un recipient. Nu îl poți deschide în timpul unei acțiuni sau urmăriri.");return false;}
  StorageCrate=index;HiddenStorage=false;Tell(index==0?"Biroul tău. H deschide ascunzătoarea de 3 obiecte, protejată la percheziție și capturare.":"Alege obiectul pe care vrei să îl iei.");return true;
 }
 public List<string> StoredItems {get {return HiddenStorage?S.Stash:StorageCrate>=0&&StorageCrate<S.Crates.Count?S.Crates[StorageCrate].Items:new List<string>();}}
 public bool ToggleStorage(){if(StorageCrate!=0||!CanOpenStorage(0))return false;HiddenStorage=!HiddenStorage;Tell(HiddenStorage?"Ascunzătoare: protejată la percheziție și capturare.":"Birou obișnuit: control zilnic la 18:30; contrabanda se confiscă.");return true;}
 public bool TakeStored(int slot){
  if(!CanOpenStorage(StorageCrate)||HiddenStorage&&StorageCrate!=0)return false;
  var items=StoredItems;if(slot<0||slot>=items.Count)return false;
  if(S.Inventory.Count>=6){Tell("Buzunarele sunt pline. Depozitează sau folosește un obiect.");return false;}
  string item=items[slot];items.RemoveAt(slot);S.Inventory.Add(item);Tell("Ai luat "+item+". Buzunare: "+S.Inventory.Count+"/6.");return true;
 }
 public bool DepositStored(int slot){
  if(StorageCrate!=0||!CanOpenStorage(0)||slot<0||slot>=S.Inventory.Count)return false;
  var items=StoredItems;int capacity=HiddenStorage?3:100;
  if(items.Count>=capacity){Tell(HiddenStorage?"Ascunzătoarea este plină: 3/3. Obiectul rămâne în buzunar.":"Biroul este plin. Obiectul rămâne în buzunar.");return false;}
  string item=S.Inventory[slot];S.Inventory.RemoveAt(slot);items.Add(item);Tell("Ai pus "+item+(HiddenStorage?" în ascunzătoare.":" în birou."));return true;
 }
}
}
