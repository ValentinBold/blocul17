using System;
using System.IO;
using System.Linq;
using System.Drawing;

namespace Blocul17 {
public static partial class Tests {
 static void StorageChecks(){
  var m=new GameModel();m.S.Inventory.Add("pilă");m.Drop(0);Check(m.S.Inventory.Contains("pilă"),"remote quick deposit refuses transfer");Check(!m.BeginStorage(),"remote container access refused");
  Walk(m,5.5,5.5);m.Interact();Check(m.StorageCrate==0&&m.S.Inventory.Count==1&&m.S.Crates[0].Items.Count==2,"interact opens container without taking first item");
  Check(m.TakeStored(1)&&m.S.Inventory.Last()=="biscuiți"&&m.S.Crates[0].Items.SequenceEqual(new[]{"bandă"}),"chosen container slot transfers exact item");Check(!m.TakeStored(-1)&&!m.TakeStored(99),"invalid container slots refused");
  while(m.S.Inventory.Count<6)m.S.Inventory.Add("pilă");int before=m.StoredItems.Count;Check(!m.TakeStored(0)&&m.StoredItems.Count==before,"full pockets preserve container contents");
  Check(m.ToggleStorage()&&m.HiddenStorage,"own desk exposes hidden compartment");Check(m.DepositStored(0)&&m.DepositStored(0)&&m.DepositStored(0)&&m.S.Stash.Count==3,"hidden compartment accepts three objects");int pockets=m.S.Inventory.Count;Check(!m.DepositStored(0)&&m.S.Inventory.Count==pockets&&m.S.Stash.Count==3,"full hidden compartment preserves pocket item");Shot(m,"stash-full","storage");
  Check(m.TakeStored(1)&&m.S.Inventory.Last()=="biscuiți"&&m.S.Stash.Count==2,"chosen hidden slot retrieved exactly");m.DepositStored(m.S.Inventory.Count-1);
  string save=Path.Combine(output,"storage-save.xml");m.Save(save);var loaded=new GameModel();Check(loaded.Load(save)&&loaded.S.Stash.SequenceEqual(m.S.Stash)&&loaded.StorageCrate==-1&&!loaded.HiddenStorage,"hidden inventory survives save with closed transient menu");
  string[] protectedItems=m.S.Stash.ToArray();m.S.Crates[0].Items.Add("clește");m.Caught();Check(m.S.Stash.SequenceEqual(protectedItems)&&m.S.Inventory.Count==0&&!m.S.Crates[0].Items.Contains("clește")&&m.StorageCrate==-1,"capture preserves stash but confiscates pockets and ordinary desk");
  Walk(m,5.5,5.5);Check(m.BeginStorage()&&m.ToggleStorage()&&m.TakeStored(0)&&m.S.Inventory[0]==protectedItems[0],"protected item recoverable after capture via physical desk");
  m.S.X=25.5;m.S.Y=5.5;Check(m.BeginStorage()&&m.StorageCrate==1&&!m.HiddenStorage&&!m.ToggleStorage(),"workshop container cannot expose personal stash");Check(!m.DepositStored(0)&&m.S.Inventory.Count==1,"foreign container cannot receive deposits");
  m.S.X=5.5;m.S.Y=5.5;Check(!m.TakeStored(0),"stale remote container selection cannot transfer");m.S.Heat=60;Check(!m.BeginStorage(),"high suspicion blocks container menu");m.S.Heat=0;m.Guards[0].Alert=1;Check(!m.BeginStorage(),"chase blocks container menu");m.Guards[0].Alert=0;m.Action="Studiu";Check(!m.BeginStorage(),"ongoing action blocks container menu");m.Action="";
  m.S.X=5.5;m.S.Y=3.5;Check(!m.BeginStorage(),"wall prevents container access even at close distance");m.S.X=5.5;m.S.Y=5.5;m.S.Health=0;Check(!m.BeginStorage(),"defeat blocks container menu");m.S.Health=100;m.S.Won=true;Check(!m.BeginStorage(),"victory blocks container menu");m.S.Won=false;
  m.BeginStorage();m.S.Crates[0].Items=Enumerable.Repeat("bandă",100).ToList();Check(!m.DepositStored(0)&&m.S.Inventory.Count==1,"ordinary desk capacity enforced through storage API");
  using(var v=new GameView())using(var b=new Bitmap(1280,900))using(var g=Graphics.FromImage(b)){v.StoragePage=16;v.Render(g,m,"storage",false);Check(v.StoragePage==16&&v.Buttons.ContainsKey("take3")&&!v.Buttons.ContainsKey("take4"),"last page renders four of hundred stored items");b.Save(Path.Combine(output,"storage-last-page.png"));}
  Check(m.TakeStored(99)&&m.S.Crates[0].Items.Count==99,"last item of full desk is retrievable");
  m.S.Stash.Add("pilă");m.S.Stash.Add("pilă");m.Save(save);Check(!loaded.Load(save),"oversize stash save rejected");m.S.Stash=new System.Collections.Generic.List<string>{"unknown"};m.Save(save);Check(!loaded.Load(save),"unknown hidden item rejected");m.S.Stash=null;m.Save(save);Check(loaded.Load(save)&&loaded.S.Stash.Count==0,"XML nil stash normalizes to safe empty inventory");
  m.Reset();Check(m.S.Stash.Count==0&&m.StorageCrate==-1,"new game resets hidden inventory");
  string fixture=Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"..","fixtures","save-v3.xml"));Check(m.Load(fixture)&&m.S.Version==5&&m.S.TunnelStage==2&&m.S.Money==19&&m.S.Inventory.Contains("lopată")&&m.S.Stash.Count==0,"genuine v3 migration preserves tunnel economy and shovel with empty stash");
  m.S.X=5.5;m.S.Y=5.5;m.BeginStorage();m.ToggleStorage();m.DepositStored(0);m.Save(save);Check(loaded.Load(save)&&loaded.S.Stash.Contains("lopată")&&loaded.S.TunnelStage==2,"migrated game round trips new hidden inventory");Shot(m,"stash","storage");
 }
}
}

