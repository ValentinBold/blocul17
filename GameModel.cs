using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Blocul17 {
public class Actor {
 public double X, Y, DX=0, DY=1,LastX,LastY;
 public int Target; public double Alert;
 public Actor() {} public Actor(double x,double y){X=x;Y=y;}
}
public class Crate {
 public string Name; public int X,Y; public List<string> Items=new List<string>();
 public Crate(){} public Crate(string n,int x,int y,params string[] items){Name=n;X=x;Y=y;Items=items.ToList();}
}
public class SaveData {
 public int Version=4,Day=1, Caught=0; public double X=5.5,Y=6.5,Minutes=420,Energy=100,Health=100,Intellect=20,Heat=0;
 public int Money=0,WorkDone=0,ActiveFavor=-1;
 public int[] FavorPaidDay=new int[3],TradeStock=new[]{2,2,2,2};
 [XmlArray(IsNullable=true)] public List<string> Stash;
 public int TunnelStage=0; public bool EscapedTunnel=false;
 public bool FenceOpen=false,Won=false; public List<string> Inventory=new List<string>(); public List<Crate> Crates=new List<Crate>(); public Actor[] Patrol,Prisoners;
}
public partial class GameModel {
 public const int W=34,H=26;
 public int[,] Tiles=new int[W,H]; // 0 grass,1 floor,2 wall,3 fence,4 path
 public SaveData S; public Actor[] Guards; public Actor[] Inmates;
 public string Message="Ajungi la apel în curtea centrală. Gardienii observă absențele.",Action="";
 public double ActionTime,ActionTotal,AutoSaveTime; public int ActionX,ActionY; public bool Changed=true;
 public readonly List<string> Log=new List<string>();
 public GameModel(){Reset();}
 public static double Dist(double x,double y,double a,double b){return Math.Sqrt((x-a)*(x-a)+(y-b)*(y-b));}
 public void Reset(){
  S=new SaveData();S.Stash=new List<string>(); S.Crates=NewCrates();Build();SetupActors();Log.Clear();Action="";ActionTime=0;TalkNpc=-1;StorageCrate=-1;HiddenStorage=false;AutoSaveTime=0;Tell("Ziua 1. Munca și favorurile pot finanța planul tău.");
 }
 public List<Crate> NewCrates(){return new List<Crate>{new Crate("Biroul tău",5,4,"bandă","biscuiți"),new Crate("Dulapul atelierului",25,4,"pilă","sârmă"),new Crate("Trusa de rezervă",28,7,"pilă","bandă"),new Crate("Masa cantinei",25,19,"biscuiți","biscuiți")};}
 void Room(int x,int y,int w,int h,int doorX,int doorY){for(int a=x;a<x+w;a++)for(int b=y;b<y+h;b++)Tiles[a,b]=(a==x||b==y||a==x+w-1||b==y+h-1)?2:1;Tiles[doorX,doorY]=1;}
 public void Build(){
  Tiles=new int[W,H];for(int x=1;x<W-1;x++){Tiles[x,1]=3;Tiles[x,H-2]=3;}for(int y=1;y<H-1;y++){Tiles[1,y]=3;Tiles[W-2,y]=3;}
  for(int x=2;x<32;x++)for(int y=11;y<15;y++)Tiles[x,y]=4;
  for(int x=14;x<20;x++)for(int y=2;y<24;y++)Tiles[x,y]=4;
  Room(3,3,10,7,6,9);for(int y=4;y<9;y++)Tiles[8,y]=2;Tiles[8,7]=1;
  Room(22,3,9,7,26,9);Room(3,16,10,7,8,16);Room(22,17,9,6,26,17);
  if(S.FenceOpen)Tiles[32,12]=4;
 }
 void SetupActors(){Guards=new[]{new Actor(17.5,9.5),new Actor(28.5,13.5)};Inmates=new[]{new Actor(16.5,12.5),new Actor(18.5,13.5),new Actor(17.5,14.5),new Actor(15.5,13.5),new Actor(19.5,12.5)};}
 public bool Solid(int x,int y){return x<0||y<0||x>=W||y>=H||Tiles[x,y]==2||Tiles[x,y]==3;}
 public bool Free(double x,double y){const double r=.23;return !Solid((int)Math.Floor(x-r),(int)Math.Floor(y-r))&&!Solid((int)Math.Floor(x+r),(int)Math.Floor(y-r))&&!Solid((int)Math.Floor(x-r),(int)Math.Floor(y+r))&&!Solid((int)Math.Floor(x+r),(int)Math.Floor(y+r));}
 public string Routine {get {double m=S.Minutes;if(m>=420&&m<480||m>=1080&&m<1140)return "APEL";if(m>=480&&m<540)return "MIC DEJUN";if(m>=540&&m<720)return "ATELIER";if(m>=1320||m<420)return "STINGEREA";return "TIMP LIBER";}}
 public string Zone {get {return ZoneAt(S.X,S.Y);}}
 public string ZoneAt(double x,double y){if(x>=3&&x<13&&y>=3&&y<10)return "CELULE";if(x>=22&&x<31&&y>=3&&y<10)return "ATELIER";if(x>=3&&x<13&&y>=16&&y<23)return "BIBLIOTECĂ";if(x>=22&&x<31&&y>=17&&y<23)return "CANTINĂ";return "CURTE";}
 public bool AtRoutine(){return Routine=="APEL"?S.X>14&&S.X<21&&S.Y>10&&S.Y<16:Routine=="MIC DEJUN"?Zone=="CANTINĂ":Routine=="STINGEREA"?Zone=="CELULE":true;}
 public bool Grace(){return Routine=="APEL"?S.Minutes%60<20:Routine=="MIC DEJUN"?S.Minutes<500:Routine=="STINGEREA"?S.Minutes>=1320&&S.Minutes<1340:true;}
 public string Clock {get{return ((int)S.Minutes/60).ToString("00")+":"+((int)S.Minutes%60).ToString("00");}}
 public void Tell(string m){Message=m;Log.Insert(0,Clock+"  "+m);if(Log.Count>20)Log.RemoveAt(20);Changed=true;}
 public bool Add(string item){if(S.Inventory.Count>=6){Tell("Buzunarele sunt pline. Consumă biscuiți sau lasă un obiect.");return false;}S.Inventory.Add(item);Changed=true;return true;}
 public Crate NearbyCrate(){return S.Crates.FirstOrDefault(c=>Dist(S.X,S.Y,c.X+.5,c.Y+.5)<1.65&&LOS(S.X,S.Y,c.X+.5,c.Y+.5));}
 public string Prompt(){
  if(Action!="")return Action+"… Mișcarea întrerupe acțiunea.";
  if(AtTunnel())return "E  ·  Trapă — tunel "+S.TunnelStage+"/3"+(S.TunnelStage==3?" · ieșire 22:00–07:00":" · lopată, 20 energie / etapă");
  if(AtWorkbench())return "E  ·  Banc de lucru — 12 cr. / piesă, 15 energie     F  ·  vorbește";
  int near=NearbyInmate();if(near>=0)return "F  ·  Vorbește cu "+NpcNames[near]+"     E  ·  interacțiune cu obiectele";
  Crate c=NearbyCrate();if(c!=null)return "E  ·  "+c.Name+" — alege obiecte";
  if(Dist(S.X,S.Y,5.5,7.5)<1.6)return "E  ·  Pat — odihnă și recuperare";
  if(Dist(S.X,S.Y,6.5,19.5)<2)return "E  ·  Citește — +10 intelect";
  if(Dist(S.X,S.Y,32,12.5)<2.2)return S.FenceOpen?"Breșa e deschisă. Mergi spre est!":"E  ·  Taie gardul — clește, 30 energie";
  return "WASD  mișcare     Shift  alergare     E  interacțiune";
 }
 public void Interact(){
  if(Action!=""||S.Won||S.Health<=0)return;
  if(AtTunnel()){UseTunnel();return;}
  if(AtWorkbench()){StartWork();return;}
  Crate c=NearbyCrate();if(c!=null){BeginStorage();return;}
  if(Dist(S.X,S.Y,5.5,7.5)<1.6){Begin("Odihnă",4);return;}
  if(Dist(S.X,S.Y,6.5,19.5)<2){Begin("Studiu",3);return;}
  if(Dist(S.X,S.Y,32,12.5)<2.2&&!S.FenceOpen){if(!S.Inventory.Contains("clește")){Tell("Îți trebuie un clește: pilă + sârmă + bandă. Deschide crafting-ul cu C.");return;}if(S.Energy<30){Tell("Ai nevoie de 30 energie. Odihnește-te în celulă.");return;}Begin("Tăierea gardului",8);Tell("Tai gardul. Oprește-te și ascunde-te dacă vine patrula.");return;}
  Tell("Apropie-te de un birou, pat, bibliotecă sau gardul marcat din est.");
 }
 void Begin(string name,double seconds){Action=name;ActionTime=0;ActionTotal=seconds;ActionX=(int)S.X;ActionY=(int)S.Y;Changed=true;}
 public bool Craft(){if(S.Won||S.Health<=0||Action!="")return false;if(S.Intellect<30){Tell("Ai nevoie de 30 intelect. Citește o dată în bibliotecă.");return false;}string[] need={"pilă","sârmă","bandă"};if(need.Any(i=>!S.Inventory.Contains(i))){Tell("Lipsesc materiale: ai nevoie de pilă, sârmă și bandă.");return false;}foreach(string i in need)S.Inventory.Remove(i);S.Inventory.Add("clește");Tell("Clește construit. Caută gardul slăbit, pe latura estică.");return true;}
 public void Use(int slot){if(slot<0||slot>=S.Inventory.Count)return;string i=S.Inventory[slot];if(i=="biscuiți"){S.Inventory.RemoveAt(slot);S.Energy=Math.Min(100,S.Energy+35);S.Health=Math.Min(100,S.Health+10);Tell("Biscuiți: +35 energie, +10 sănătate.");}else Tell(i=="lopată"?"Folosește lopata la trapa din bibliotecă, cu E.":i=="clește"?"Folosește cleștele lângă gardul marcat, cu E.":"Material de crafting. Apasă C pentru rețetă.");}
 public void Drop(int slot){if(!CanOpenStorage(0)){Tell("Depozitarea cere apropierea de biroul tău, în afara urmăririi.");return;}if(slot<0||slot>=S.Inventory.Count)return;if(S.Crates[0].Items.Count>=100){Tell("Biroul este plin. Obiectul rămâne în buzunar.");return;}string i=S.Inventory[slot];S.Inventory.RemoveAt(slot);S.Crates[0].Items.Add(i);Tell("Ai pus "+i+" în biroul tău. Îl poți recupera.");}
 public bool LOS(double x,double y,double tx,double ty){double d=Dist(x,y,tx,ty);int steps=(int)(d*12)+1;for(int i=1;i<steps;i++){double t=(double)i/steps;if(Solid((int)(x+(tx-x)*t),(int)(y+(ty-y)*t)))return false;}return true;}
 public bool Sees(Actor g){double d=Dist(g.X,g.Y,S.X,S.Y);if(d>6.5||!LOS(g.X,g.Y,S.X,S.Y))return false;if(d<1.25)return true;return ((S.X-g.X)*g.DX+(S.Y-g.Y)*g.DY)/d>.32;}
 public int[] NextTile(double x,double y,double tx,double ty){
  int sx=(int)x,sy=(int)y,ex=(int)tx,ey=(int)ty;if(Solid(ex,ey))return null;
  var q=new Queue<int>();var prev=new int[W*H];for(int i=0;i<prev.Length;i++)prev[i]=-1;int start=sy*W+sx,goal=ey*W+ex;prev[start]=start;q.Enqueue(start);int[] dx={1,0,-1,0},dy={0,1,0,-1};
  while(q.Count>0){int at=q.Dequeue();if(at==goal)break;for(int k=0;k<4;k++){int nx=at%W+dx[k],ny=at/W+dy[k];if(Solid(nx,ny))continue;int n=ny*W+nx;if(prev[n]>=0)continue;prev[n]=at;q.Enqueue(n);}}
  if(prev[goal]<0)return null;int step=goal;while(prev[step]!=start&&step!=start)step=prev[step];return new[]{step%W,step/W};
 }
 public void MoveActor(Actor a,double tx,double ty,double speed,double dt){
  if(Dist(a.X,a.Y,tx,ty)<.1)return;
  int[] n=NextTile(a.X,a.Y,tx,ty);if(n==null)return;
  double gx=n[0]+.5,gy=n[1]+.5;if((int)a.X==(int)tx&&(int)a.Y==(int)ty){gx=tx;gy=ty;}
  // Align within the current tile before turning, avoiding diagonal corner clipping.
  if(n[0]!=(int)a.X&&Math.Abs(a.Y-((int)a.Y+.5))>.09){gx=(int)a.X+.5;gy=(int)a.Y+.5;}
  if(n[1]!=(int)a.Y&&Math.Abs(a.X-((int)a.X+.5))>.09){gx=(int)a.X+.5;gy=(int)a.Y+.5;}
  double d=Dist(a.X,a.Y,gx,gy);if(d<.005)return;double vx=(gx-a.X)/d,vy=(gy-a.Y)/d;a.DX=vx;a.DY=vy;double step=Math.Min(d,speed*dt);if(Free(a.X+vx*step,a.Y))a.X+=vx*step;if(Free(a.X,a.Y+vy*step))a.Y+=vy*step;
 }
 public void Caught(){S.Caught++;S.Health=Math.Max(0,S.Health-25);S.Energy=100;S.Heat=0;S.X=5.5;S.Y=6.5;S.Inventory.Clear();S.Crates=NewCrates();S.FenceOpen=false;S.TunnelStage=0;S.EscapedTunnel=false;Build();SetupActors();Action="";TalkNpc=-1;StorageCrate=-1;HiddenStorage=false;Tell(S.Health<=0?"Transfer disciplinar. Planul tău se încheie aici.":"Prins! Obiectele au fost confiscate. Tunelul a fost astupat; banii și favorurile rămân.");}
 public void Tick(double dt,double mx,double my,bool sprint){
  if(S.Won||S.Health<=0)return;dt=Math.Min(.05,Math.Max(0,dt));string old=Routine;S.Minutes+=dt*2;
  if(S.Minutes>=1440){S.Minutes-=1440;S.Day++;S.WorkDone=0;S.TradeStock=new[]{2,2,2,2};foreach(Crate c in S.Crates.Where(c=>c.Name=="Masa cantinei")){if(c.Items.Count==0)c.Items.Add("biscuiți");}Tell("O nouă zi. Munca, favorurile și stocul lui Radu sunt disponibile.");}
  if(old!=Routine)Tell(Routine=="APEL"?"Apel în curtea centrală. Ai 20 de minute să ajungi.":Routine=="MIC DEJUN"?"Micul dejun: mergi în cantină.":Routine=="STINGEREA"?"Stingerea. Întoarce-te la celulă sau evită patrulele.":"Program nou: "+Routine.ToLower()+".");
  double len=Math.Sqrt(mx*mx+my*my);if(len>0){mx/=len;my/=len;double speed=sprint&&S.Energy>5?4.5:2.8;if(speed>3)S.Energy=Math.Max(0,S.Energy-dt*5);double nx=S.X+mx*speed*dt,ny=S.Y+my*speed*dt;if(Free(nx,S.Y))S.X=nx;if(Free(S.X,ny))S.Y=ny;if(Action!=""){Action="";Tell("Acțiune întreruptă.");}}
  else if(Action=="")S.Energy=Math.Min(100,S.Energy+dt*.8);
  if(Action=="Muncă"&&Routine!="ATELIER"){Action="";Tell("Atelierul s-a închis. Piesa neterminată nu este plătită.");}
  if(Action!=""){ActionTime+=dt;if(ActionTime>=ActionTotal){string done=Action;Action="";if(done=="Săparea tunelului"){FinishDig();}else if(done=="Muncă"){FinishWork();}else if(done=="Studiu"){S.Intellect=Math.Min(100,S.Intellect+10);Tell("Ai citit: +10 intelect. Clește: 30, lopată: 40.");}else if(done=="Odihnă"){S.Energy=100;S.Health=Math.Min(100,S.Health+25);Tell("Te-ai odihnit. Energie refăcută, +25 sănătate.");}else if(done=="Tăierea gardului"){S.FenceOpen=true;S.Energy-=30;Build();Tell("Gardul este tăiat. Treci prin breșă și mergi spre est!");}}}
  bool infraction=Action=="Săparea tunelului"||Action=="Tăierea gardului"||(Routine=="STINGEREA"&&Zone!="CELULE")||(Zone=="ATELIER"&&Routine!="ATELIER"&&Routine!="TIMP LIBER");
  if(!AtRoutine()&&!Grace())S.Heat=Math.Min(100,S.Heat+dt*1.8);else S.Heat=Math.Max(0,S.Heat-dt*1.1);
  double[,] route={{17.5,4.5},{20.5,12.5},{29.5,13.5},{19.5,23.5},{14.5,13.5}};
  for(int i=0;i<Guards.Length;i++){Actor g=Guards[i];bool seen=Sees(g);if(seen&&infraction)S.Heat=Math.Min(100,S.Heat+dt*24);if(seen&&S.Heat>=60){g.Alert=10;}else if(g.Alert>0)g.Alert=Math.Max(0,g.Alert-dt);if(seen&&g.Alert>0){g.LastX=S.X;g.LastY=S.Y;}
   if(g.Alert>0){MoveActor(g,g.LastX,g.LastY,3.3,dt);if(Dist(g.X,g.Y,S.X,S.Y)<.65&&LOS(g.X,g.Y,S.X,S.Y)){Caught();return;}}
   else{int r=(g.Target+i*2)%5;MoveActor(g,route[r,0],route[r,1],1.55,dt);if(Dist(g.X,g.Y,route[r,0],route[r,1])<.2)g.Target++;}}
  for(int i=0;i<Inmates.Length;i++){double tx=15.5+i,ty=12.5+(i%2);if(Routine=="MIC DEJUN"){tx=24.5+i;ty=20.5;}else if(Routine=="STINGEREA"){tx=9.5+i%2;ty=5.5+i/2;}else if(Routine=="ATELIER"){tx=24.5+i;ty=6.5;}MoveActor(Inmates[i],tx,ty,1.35,dt);}
  if(S.FenceOpen&&S.X>33.1){S.Won=true;Tell("Ai trecut de ultimul gard. Ești liber.");}AutoSaveTime+=dt;Changed=true;
 }
 public string Objective {get {if(S.TunnelStage==3)return Routine=="STINGEREA"?"Intră în trapa bibliotecii · E":"Tunel gata. Ieșire după 22:00";if(S.Inventory.Contains("lopată"))return "Sapă la trapă · "+S.TunnelStage+"/3 etape";if(S.FenceOpen)return "Treci prin breșa din est";if(!AtRoutine()&&Routine=="APEL")return "Mergi la apel, în curte";if(!AtRoutine()&&Routine=="MIC DEJUN")return "Mergi la cantină";if(S.Inventory.Contains("clește"))return "Taie gardul din est";if(S.Intellect<30)return "Citește în bibliotecă";if(new[]{"pilă","sârmă","bandă"}.All(i=>S.Inventory.Contains(i)))return "Alege unealta · C";return "Găsește pilă, sârmă și bandă";}}
 public void Save(string path){Directory.CreateDirectory(Path.GetDirectoryName(path));S.Patrol=Guards;S.Prisoners=Inmates;string temp=path+".tmp";using(var f=File.Create(temp))new XmlSerializer(typeof(SaveData)).Serialize(f,S);if(File.Exists(path))File.Replace(temp,path,null);else File.Move(temp,path);AutoSaveTime=0;}
 public bool Load(string path){try{SaveData d;using(var f=File.OpenRead(path))d=(SaveData)new XmlSerializer(typeof(SaveData)).Deserialize(f);if(d==null||(d.Version<1||d.Version>4)||d.Inventory==null||d.Inventory.Count>6||d.Crates==null||d.Crates.Count!=4||d.Day<1||d.Day>100000)return false;
   if(d.Version==1){d.Money=0;d.WorkDone=0;d.ActiveFavor=-1;d.FavorPaidDay=new int[3];d.TradeStock=new[]{2,2,2,2};d.Version=2;}
   if(d.Version<3){d.TunnelStage=0;d.EscapedTunnel=false;d.Version=3;}
   if(d.Version<4){d.Stash=new List<string>();d.Version=4;}
   if(d.TunnelStage<0||d.TunnelStage>3||d.EscapedTunnel&&(!d.Won||d.TunnelStage!=3))return false;
   if(!ValidEconomy(d))return false;
   double[] nums={d.X,d.Y,d.Minutes,d.Energy,d.Health,d.Intellect,d.Heat};if(nums.Any(n=>Double.IsNaN(n)||Double.IsInfinity(n))||d.X<.25||d.X>W-.25||d.Y<.25||d.Y>H-.25||d.Minutes<0||d.Minutes>=1440||d.Energy<0||d.Energy>100||d.Health<0||d.Health>100||d.Intellect<0||d.Intellect>100||d.Heat<0||d.Heat>100)return false;
   string[] valid={"pilă","sârmă","bandă","biscuiți","clește","lopată"};if(d.Stash==null||d.Stash.Count>3||d.Stash.Any(i=>!valid.Contains(i)))return false;
   if(d.Caught<0||d.Inventory.Any(i=>!valid.Contains(i))||d.Crates.Any(c=>c==null||c.Items==null||c.Items.Count>100||c.Items.Any(i=>!valid.Contains(i))))return false;
   var expected=NewCrates();for(int i=0;i<expected.Count;i++)if(d.Crates[i].Name!=expected[i].Name||d.Crates[i].X!=expected[i].X||d.Crates[i].Y!=expected[i].Y)return false;
   if(d.Patrol==null||d.Patrol.Length!=2||d.Prisoners==null||d.Prisoners.Length!=5)return false;
   var actors=d.Patrol.Concat(d.Prisoners);if(actors.Any(a=>a==null||new[]{a.X,a.Y,a.DX,a.DY,a.LastX,a.LastY,a.Alert}.Any(v=>Double.IsNaN(v)||Double.IsInfinity(v))||a.X<.25||a.X>W-.25||a.Y<.25||a.Y>H-.25||a.Alert<0||a.Alert>10||a.Target<0||a.Target>10000000))return false;
   var original=S;S=d;Build();if(!Free(S.X,S.Y)||actors.Any(a=>!Free(a.X,a.Y))){S=original;Build();return false;}Guards=d.Patrol;Inmates=d.Prisoners;Action="";TalkNpc=-1;StorageCrate=-1;HiddenStorage=false;Tell("Partidă încărcată. Planul continuă.");return true;
  }catch{return false;}}
}
}


