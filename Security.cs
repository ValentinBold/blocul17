using System;
using System.Collections.Generic;
using System.Linq;

namespace Blocul17 {
public partial class GameModel {
 public static readonly string[] Contraband={"pilă","sârmă","clește","lopată"};
 public static readonly int[] RecoveryPrices={8,6,24,28};
 public bool EvidenceOpen=false;
 public static bool IsContraband(string item){return Array.IndexOf(Contraband,item)>=0;}
 public bool MaterialPermit {get {return S.Minutes>=540&&S.Minutes<720;}}
 public bool OnScanner {get {return (int)S.X==26&&(int)S.Y==9;}}
 public int PocketRisk {get {return S.Inventory.Count(IsContraband);}}
 public bool AtEvidence(){return Dist(S.X,S.Y,28.5,15.5)<1.3&&LOS(S.X,S.Y,28.5,15.5);}
 public bool CanRecover(){return AtEvidence()&&S.Minutes>=720&&S.Minutes<1320&&!S.Won&&S.Health>0&&Action==""&&S.Heat<60&&!Guards.Any(a=>a.Alert>0);}
 public bool BeginEvidence(){
  if(!CanRecover()){Tell("Ghișeu: 12:00–22:00. Apropie-te fără urmărire, fără acțiune și cu suspiciune sub 60.");return false;}
  EvidenceOpen=true;Tell("Recuperează un obiect confiscat, contra cost. Timpul este oprit în meniu.");return true;
 }
 public bool RecoverEvidence(int item){
  if(!EvidenceOpen||!CanRecover()||item<0||item>=Contraband.Length)return false;
  if(S.Evidence[item]<=0){Tell("Nu ai acest obiect în depozitul de confiscări.");return false;}
  if(S.Money<RecoveryPrices[item]){Tell("Nu ai suficiente credite pentru recuperare.");return false;}
  if(S.Inventory.Count>=6){Tell("Buzunarele sunt pline. Obiectul rămâne în depozit.");return false;}
  S.Evidence[item]--;S.Money-=RecoveryPrices[item];S.Inventory.Add(Contraband[item]);Tell("Ai recuperat "+Contraband[item]+" pentru "+RecoveryPrices[item]+" credite.");return true;
 }
 // Each counter is bounded; excess only arises from extreme or edited saves.
 int Confiscate(List<string> items){
  int count=0;for(int i=items.Count-1;i>=0;i--){int kind=Array.IndexOf(Contraband,items[i]);if(kind<0)continue;
   if(S.Evidence[kind]>=1000000)continue; // Keep the item rather than silently discard it.
   S.Evidence[kind]++;items.RemoveAt(i);count++;
  }return count;
 }
 void CaptureEvidence(){Confiscate(S.Inventory);Confiscate(S.Crates[0].Items);}
 void ScanCrossing(bool wasOnScanner){
  if(wasOnScanner||!OnScanner)return;
  bool detected=S.Inventory.Any(i=>IsContraband(i)&&(!MaterialPermit||i=="clește"||i=="lopată"));
  if(!detected)return;
  S.Heat=Math.Min(100,S.Heat+35);Waiting=false;Tell("Detector: contrabandă! +35 suspiciune. Materialele simple sunt permise doar 09:00–12:00.");
 }
 void SecurityEvents(double previousMinutes,int previousDay){
  if(previousDay!=S.Day)return;
  if(previousMinutes<1080&&S.Minutes>=1080&&S.InspectionNoticeDay!=S.Day){
   S.InspectionNoticeDay=S.Day;Waiting=false;Tell("Anunț: biroul tău va fi percheziționat la 18:30. Ascunzătoarea rămâne protejată.");
  }
  if(previousMinutes<1110&&S.Minutes>=1110&&S.InspectionDay!=S.Day){
   S.InspectionDay=S.Day;S.LastInspectionCount=Confiscate(S.Crates[0].Items);Waiting=false;
   if(S.LastInspectionCount>0)S.Heat=Math.Min(100,S.Heat+20);
   Tell(S.LastInspectionCount>0?"Percheziție: "+S.LastInspectionCount+" obiecte în depozit. +20 suspiciune. Recuperare la ghișeu, 12:00–22:00.":"Percheziție încheiată: nimic interzis în birou. Ascunzătoarea este intactă.");
  }
 }
 public string InspectionStatus {get {return S.InspectionDay==S.Day?"Azi: control încheiat · "+S.LastInspectionCount+" confiscate":S.Minutes<1110?"Azi, 18:30 · control al biroului":"Următorul control: mâine, 18:30";}}
 static bool ValidSecurity(SaveData d){return d.Evidence!=null&&d.Evidence.Length==4&&d.Evidence.All(n=>n>=0&&n<=1000000)&&d.InspectionDay>=0&&d.InspectionDay<=d.Day&&d.InspectionNoticeDay>=0&&d.InspectionNoticeDay<=d.Day&&d.LastInspectionCount>=0&&d.LastInspectionCount<=100;}
}
}
