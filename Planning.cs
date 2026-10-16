using System;
using System.Linq;

namespace Blocul17 {
public partial class GameModel {
 public static readonly string[] PlanItems={"pilă","sârmă","bandă","clește","lopată"};
 public static readonly int[] ScheduleTimes={420,480,540,720,1080,1110,1140,1320,1440};
 public static readonly string[] ScheduleNames={"Apel în curte","Mic dejun în cantină","Atelier și permis materiale","Ghișeu deschis · permis expirat","Apel · anunț de percheziție","Percheziția biroului","Timp liber","Stingerea · ieșire prin tunel","Zi nouă · stocuri refăcute"};
 // Counts describe ownership, not access. Stored and confiscated items cannot be crafted remotely.
 public int[] PlanStock(string item){int n=Array.IndexOf(Contraband,item);return new[]{S.Inventory.Count(i=>i==item),S.Crates[0].Items.Count(i=>i==item),S.Stash.Count(i=>i==item),n<0?0:S.Evidence[n]};}
 public string NextScheduleEvent {get {int n=Array.FindIndex(ScheduleTimes,t=>t>S.Minutes);if(n<0)n=ScheduleTimes.Length-1;return ScheduleClock(ScheduleTimes[n])+" · "+ScheduleNames[n];}}
 public static string ScheduleClock(int minutes){return (minutes==1440?"00:00 (+1 zi)":(minutes/60).ToString("00")+":"+(minutes%60).ToString("00"));}
 public string PlanProgress(bool tunnel){return tunnel?"Tunel: "+S.TunnelStage+"/3 etape · ieșire 22:00–07:00":S.FenceOpen?"Gard deschis · treci prin breșa din est":"Gard intact · tăiere 8 secunde / 30 energie";}
 string RetrieveAdvice(string item){
  int[] stock=PlanStock(item);
  if(stock[2]>0)return "Ia "+item+" din ascunzătoare: E la biroul tău, H, apoi alege obiectul.";
  if(stock[1]>0)return "Ia "+item+" din biroul tău: E, apoi alege obiectul. Nu poți folosi materiale de la distanță.";
  if(stock[3]>0){int n=Array.IndexOf(Contraband,item);int price=RecoveryPrices[n];return "Recuperează "+item+" la ghișeul de lângă cantină, 12:00–22:00: "+price+" cr."+(S.Money<price?" Îți lipsesc "+(price-S.Money)+" cr.; lucrează sau fă favoruri.":S.Inventory.Count>=6?" Eliberează un loc în buzunar.":" Ai banii necesari.");}
  return "";
 }
 public string PlanAdvice(bool tunnel){
  if(S.Won)return "Ai evadat. O partidă nouă începe un plan de la zero.";
  if(S.Health<=0)return "Transfer disciplinar. Începe o partidă nouă din ecranul final.";
  if(Guards.Any(a=>a.Alert>0)||S.Heat>=60)return "Pierde urmărirea după ziduri și lasă suspiciunea să scadă înainte de următorul pas.";
  if(!tunnel&&S.FenceOpen)return "Gardul este deschis. Mergi prin breșa din est, dincolo de incintă.";
  if(tunnel&&S.TunnelStage==3)return Routine=="STINGEREA"?"Tunel gata: E la trapa din sud-estul bibliotecii. Nu mai ai nevoie de lopată.":"Tunel gata. Revino la trapa bibliotecii după 22:00; nu mai ai nevoie de lopată.";
  if(!AtRoutine())return "Mai întâi respectă programul: "+(Routine=="APEL"?"mergi în curtea centrală.":Routine=="MIC DEJUN"?"mergi la cantină.":"întoarce-te în celulă sau evită patrulele.");
  if(Action!="")return "Acțiune în curs: "+Action+". Închide planul ca să continui; mișcarea o anulează.";
  if(S.Minutes>=1080&&S.Minutes<1110&&S.InspectionDay!=S.Day&&S.Crates[0].Items.Any(IsContraband))return "Control la 18:30: mută contrabanda din birou în ascunzătoare (3 locuri) sau în buzunare.";
  string tool=tunnel?"lopată":"clește";
  if(S.Inventory.Contains(tool))return S.Energy<(tunnel?20:30)?"Nu ai energie suficientă. Odihnește-te în pat sau mănâncă biscuiți.":tunnel?"E la trapa din sud-estul bibliotecii: sapă etapa "+(S.TunnelStage+1)+"/3. Ferește-te de patrule.":"E lângă gardul marcat din est. Așteaptă să treacă patrula înainte de a tăia.";
  string stored=RetrieveAdvice(tool);if(stored!="")return stored;
  int target=tunnel?40:30;if(S.Intellect<target)return "Citește în bibliotecă: mai ai nevoie de "+(int)Math.Ceiling((target-S.Intellect)/10)+(target-S.Intellect<=10?" lectură pentru ":" lecturi pentru ")+target+" intelect.";
  foreach(string item in PlanItems.Take(3)){if(S.Inventory.Contains(item))continue;stored=RetrieveAdvice(item);if(stored!="")return stored;return "Îți lipsește "+item+". Caută în "+(item=="bandă"?"birou sau trusa de rezervă":"dulapul atelierului")+" ori cumpără de la Radu. Pila și sârma au permis 09:00–12:00.";}
  return "Ai intelectul și materialele în buzunare. Apasă C, apoi "+(tunnel?"2 pentru lopată.":"1 pentru clește.");
 }
 public string VeteranAdvice {get {return PlanAdvice(S.TunnelStage>0||PlanStock("lopată").Sum()>0);}}
}
}
