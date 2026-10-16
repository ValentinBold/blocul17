using System;
using System.Linq;

namespace Blocul17 {
public partial class GameModel {
 public static readonly string[] NpcNames={"Matei","Radu","Ion","Dima","Victor"};
 public static readonly string[] NpcRoles={"Curierul","Negustorul","Mecanicul","Electricianul","Veteranul"};
 public static readonly string[] ShopItems={"bandă","pilă","sârmă","biscuiți"};
 public static readonly int[] BuyPrices={8,12,10,6},SellPrices={4,6,5,3};
 public static readonly string[] FavorItems={"biscuiți","bandă","sârmă"};
 public static readonly int[] FavorNpcs={0,2,3},FavorRewards={18,14,16};
 public int TalkNpc=-1;
 public bool AtWorkbench(){return Dist(S.X,S.Y,24.5,6.5)<1.1&&LOS(S.X,S.Y,24.5,6.5);}
 public bool StartWork(){
  if(S.Won||S.Health<=0||Action!="")return false;
  if(!AtWorkbench()){Tell("Munca se face la bancul din stânga atelierului.");return false;}
  if(Routine!="ATELIER"){Tell("Bancul este deschis între 09:00 și 12:00.");return false;}
  if(S.WorkDone>=3){Tell("Ai terminat cele 3 piese plătite de astăzi. Revino mâine.");return false;}
  if(S.Energy<15){Tell("O piesă cere 15 energie. Odihnește-te sau mănâncă.");return false;}
  if(S.Money>999988){Tell("Portofelul este plin.");return false;}
  Begin("Muncă",6);Tell("Finisezi o piesă. Rămâi aici 6 secunde pentru 12 credite.");return true;
 }
 void FinishWork(){
  if(Routine!="ATELIER"||!AtWorkbench()||S.WorkDone>=3||S.Energy<15)return;
  S.WorkDone++;S.Money+=12;S.Energy-=15;Tell("Piesă predată: +12 credite. Schimbul de azi: "+S.WorkDone+"/3.");
 }
 public int NearbyInmate(){
  int best=-1;double distance=1.7;
  for(int i=0;i<Inmates.Length;i++){Actor a=Inmates[i];double d=Dist(S.X,S.Y,a.X,a.Y);if(d<distance&&LOS(S.X,S.Y,a.X,a.Y)){best=i;distance=d;}}
  return best;
 }
 public bool CanTalkTo(int npc){return npc>=0&&npc<Inmates.Length&&!S.Won&&S.Health>0&&Action==""&&S.Heat<60&&!Guards.Any(g=>g.Alert>0)&&Dist(S.X,S.Y,Inmates[npc].X,Inmates[npc].Y)<1.7&&LOS(S.X,S.Y,Inmates[npc].X,Inmates[npc].Y);}
 public bool BeginTalk(){
  int n=NearbyInmate();if(n<0){Tell("Apropie-te de un deținut. F deschide conversația.");return false;}
  if(!CanTalkTo(n)){Tell("Nu poți negocia în timpul unei acțiuni sau când ești urmărit.");return false;}
  TalkNpc=n;Tell(NpcNames[n]+": "+Dialogue(n));return true;
 }
 public int FavorForNpc(int npc){return Array.IndexOf(FavorNpcs,npc);}
 public string Dialogue(int npc){
  if(npc==1)return "Am materiale. Tu ai credite? Pot cumpăra și ce nu-ți mai trebuie.";
  if(npc==4)return VeteranAdvice;
  int favor=FavorForNpc(npc);if(favor<0)return "";
  if(S.FavorPaidDay[favor]==S.Day)return "Suntem chit pentru azi. Revino mâine.";
  if(S.ActiveFavor==favor)return "Aștept "+FavorItems[favor]+". Ai "+FavorRewards[favor]+" credite când îmi aduci obiectul.";
  return "Îmi trebuie "+FavorItems[favor]+". Îți dau "+FavorRewards[favor]+" credite dacă mă ajuți.";
 }
 public bool AcceptFavor(){
  if(!CanTalkTo(TalkNpc))return false;int f=FavorForNpc(TalkNpc);if(f<0)return false;
  if(S.FavorPaidDay[f]==S.Day){Tell("Acest favor a fost deja plătit astăzi.");return false;}
  if(S.ActiveFavor>=0){Tell("Ai deja un favor activ. Termină-l sau renunță din jurnal.");return false;}
  S.ActiveFavor=f;Tell("Favor acceptat: adu "+FavorItems[f]+" lui "+NpcNames[TalkNpc]+". Recompensă: "+FavorRewards[f]+" cr.");return true;
 }
 public bool DeliverFavor(){
  if(!CanTalkTo(TalkNpc))return false;int f=FavorForNpc(TalkNpc);
  if(f<0||S.ActiveFavor!=f||S.FavorPaidDay[f]==S.Day)return false;
  if(!S.Inventory.Contains(FavorItems[f])){Tell("Obiectul cerut trebuie să fie în buzunare: "+FavorItems[f]+".");return false;}
  if(S.Money>1000000-FavorRewards[f])return false;
  S.Inventory.Remove(FavorItems[f]);S.Money+=FavorRewards[f];S.FavorPaidDay[f]=S.Day;S.ActiveFavor=-1;
  Tell("Favor îndeplinit: +"+FavorRewards[f]+" credite. "+NpcNames[TalkNpc]+" îți mulțumește.");return true;
 }
 public void AbandonFavor(){if(S.ActiveFavor<0)return;S.ActiveFavor=-1;Tell("Ai renunțat la favor. Obiectele rămân în buzunare.");}
 public string FavorSummary {get {return S.ActiveFavor<0?"Niciun favor activ. Vorbește cu Matei, Ion sau Dima.":"Adu "+FavorItems[S.ActiveFavor]+" lui "+NpcNames[FavorNpcs[S.ActiveFavor]]+" · "+FavorRewards[S.ActiveFavor]+" cr.";}}
 public bool Buy(int item){
  if(TalkNpc!=1||!CanTalkTo(1)||item<0||item>=ShopItems.Length)return false;
  if(S.TradeStock[item]<=0){Tell("Radu nu mai are acest obiect. Stocul se reface la miezul nopții.");return false;}
  if(S.Money<BuyPrices[item]){Tell("Nu ai suficiente credite. Lucrează sau îndeplinește un favor.");return false;}
  if(S.Inventory.Count>=6){Tell("Buzunarele sunt pline. Vinde sau depozitează un obiect.");return false;}
  S.Money-=BuyPrices[item];S.TradeStock[item]--;S.Inventory.Add(ShopItems[item]);Tell("Ai cumpărat "+ShopItems[item]+" pentru "+BuyPrices[item]+" credite.");return true;
 }
 public int SalePrice(int slot){return slot<0||slot>=S.Inventory.Count?0:PriceFor(S.Inventory[slot]);}
 int PriceFor(string item){int n=Array.IndexOf(ShopItems,item);return n<0?0:SellPrices[n];}
 public bool Sell(int slot){
  if(TalkNpc!=1||!CanTalkTo(1)||slot<0||slot>=S.Inventory.Count)return false;
  string item=S.Inventory[slot];int n=Array.IndexOf(ShopItems,item);if(n<0){Tell("Radu cumpără materiale și mâncare, nu cleștele tău.");return false;}
  if(S.TradeStock[n]>=99||S.Money>1000000-SellPrices[n])return false;
  S.Inventory.RemoveAt(slot);S.TradeStock[n]++;S.Money+=SellPrices[n];Tell("Ai vândut "+item+" pentru "+SellPrices[n]+" credite.");return true;
 }
 static bool ValidEconomy(SaveData d){return d.Money>=0&&d.Money<=1000000&&d.WorkDone>=0&&d.WorkDone<=3&&d.ActiveFavor>=-1&&d.ActiveFavor<3&&d.FavorPaidDay!=null&&d.FavorPaidDay.Length==3&&d.FavorPaidDay.All(day=>day>=0&&day<=d.Day)&&d.TradeStock!=null&&d.TradeStock.Length==4&&d.TradeStock.All(n=>n>=0&&n<=99)&&(d.ActiveFavor<0||d.FavorPaidDay[d.ActiveFavor]!=d.Day);}
}
}
