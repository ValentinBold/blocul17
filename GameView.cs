using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;

namespace Blocul17 {
public partial class GameView : IDisposable {
 public const int Width=1280,Height=900,MapX=28,MapY=132,T=24;
 public Dictionary<string,Rectangle> Buttons=new Dictionary<string,Rectangle>();
 public string Hover=""; public int Selected=-1;
 readonly Dictionary<string,Font> fonts=new Dictionary<string,Font>();
 static Color C(string hex){return ColorTranslator.FromHtml(hex);}
 readonly Color Ink=C("#E9E4D5"),Muted=C("#ADB8B1"),Dark=C("#142528"),Panel=C("#1E3335"),Orange=C("#E7AA61"),Green=C("#A9C99D");
 Graphics g;
 Font F(int n,bool bold=false){string k=n+"/"+bold;if(!fonts.ContainsKey(k))fonts[k]=new Font("Segoe UI",n,bold?FontStyle.Bold:FontStyle.Regular,GraphicsUnit.Pixel);return fonts[k];}
 void Box(Color c,float x,float y,float w,float h){using(var b=new SolidBrush(c))g.FillRectangle(b,x,y,w,h);}
 void Line(Color c,float x,float y,float xx,float yy,float width=1){using(var p=new Pen(c,width))g.DrawLine(p,x,y,xx,yy);}
 void Text(string s,float x,float y,int size,Color color,bool bold=false){using(var b=new SolidBrush(color))g.DrawString(s,F(size,bold),b,x,y);}
 void Wrap(string s,float x,float y,float w,float h,int size,Color color,bool bold=false){using(var b=new SolidBrush(color))g.DrawString(s,F(size,bold),b,new RectangleF(x,y,w,h));}
 void Center(string s,int x,int y,int w,int size,Color color,bool bold=false){SizeF z=g.MeasureString(s,F(size,bold));Text(s,x+(w-z.Width)/2,y,size,color,bold);}
 void Button(string id,string label,int x,int y,int w,int h,bool primary=false,bool enabled=true){Rectangle r=new Rectangle(x,y,w,h);if(enabled)Buttons[id]=r;Box(!enabled?C("#293E40"):primary?Orange:Hover==id?C("#365254"):C("#2D4546"),x,y,w,h);if(Hover==id&&enabled)using(var p=new Pen(Ink,2))g.DrawRectangle(p,r);Center(label,x,y+(h-size(label))/2-2,w,17,!enabled?Muted:primary?Dark:Ink,true);}
 int size(string s){return 20;}
 void Meter(string label,double value,int x,int y,Color c){Text(label,x,y,14,Muted);Text(((int)value).ToString(),x+275,y,14,Ink,true);Box(C("#304747"),x,y+25,310,5);Box(c,x,y+25,(float)(310*Math.Max(0,Math.Min(100,value))/100),5);}
 public string Hit(int x,int y){foreach(var b in Buttons)if(b.Value.Contains(x,y))return b.Key;return "";}
 public void Render(Graphics graphics,GameModel m,string screen,bool hasSave,bool savingFailed=false){
  g=graphics;g.SmoothingMode=SmoothingMode.None;g.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;Buttons.Clear();g.Clear(Dark);
  Text("BLOCUL",28,23,15,Muted,true);Text("17",96,9,44,Ink,true);Line(C("#405553"),157,26,157,71);
  Text("PENITENCIARUL NORD",183,24,14,Muted,true);Text("O rutină perfectă. Un plan imperfect.",183,46,17,Ink);
  Text("ZIUA "+m.S.Day.ToString("00"),600,24,14,Muted,true);Text(m.Clock,599,41,29,m.Waiting?Orange:Ink,true);
  Box(Orange,738,28,4,40);Text(m.Routine,756,24,17,Orange,true);Text(m.AtRoutine()?"În program":"Deplasare necesară",756,49,14,Muted);
  Button("wait",m.Waiting?"T · Oprește ×20":"T · Așteaptă",888,28,150,42,m.Waiting,m.Waiting||m.CanWait());Button("guide","F1  Ghid",1050,28,99,42);Button("pause","II",1161,28,91,42);
  Line(C("#3A4E4B"),28,94,1252,94);Text("01 / INCINTA",28,107,12,Muted,true);Text(m.Zone,711,107,12,Orange,true);Text("02 / PLANUL TĂU",878,107,12,Muted,true);
  DrawMap(m);
  Box(Panel,876,132,376,624);
  Text("DEȚINUT #017",902,154,13,Muted,true);Text("O ieșire există.",902,178,29,Ink,true);
  Text(m.S.Money+" cr.",1125,152,19,Orange,true);
  Meter("SĂNĂTATE",m.S.Health,902,236,Green);Meter("ENERGIE",m.S.Energy,902,283,Orange);Meter("INTELECT",m.S.Intellect,902,330,C("#8BBAC1"));Meter("SUSPICIUNE",m.S.Heat,902,377,m.S.Heat>=60?C("#EF8875"):Orange);
  Line(C("#3A5150"),902,429,1226,429);Text("URMĂTORUL PAS",902,451,12,Orange,true);Wrap(m.Objective,902,478,305,63,23,Ink,true);
  bool tunnel=m.S.Inventory.Contains("lopată")||m.S.TunnelStage>0;
  Text(tunnel?"RUTA PRIN TUNEL":"RUTA PRIN GARD",902,557,12,Muted,true);
  Text((m.S.Intellect>=(tunnel?40:30)?"✓":"01")+"  Citește în bibliotecă",902,588,16,m.S.Intellect>=(tunnel?40:30)?Green:Ink);
  Text(tunnel?"02  Tunel: "+m.S.TunnelStage+"/3 etape":(m.S.Inventory.Contains("clește")?"✓":"02")+"  Construiește un clește",902,618,16,Ink);
  Text(tunnel?"03  Ieși după 22:00 · E":(m.S.FenceOpen?"✓":"03")+"  Deschide gardul din est",902,648,16,Ink);
  Button("security","K · Securitate",902,672,155,22);Text("F · vorbește",1072,675,12,Muted);
  Button("craft","C  Construiește",902,698,155,36);Button("journal","J  Jurnal",1069,698,157,36);
  Text("03 / BUZUNARE",28,774,12,Muted,true);Text("Click: folosește   ·   Click dreapta: depune lângă birou",287,774,12,Muted);
  for(int i=0;i<6;i++){int x=28+i*137;Box(i==Selected?C("#3F5752"):Panel,x,797,126,69);Buttons["item"+i]=new Rectangle(x,797,126,69);Text((i+1).ToString(),x+9,803,12,Muted);if(i<m.S.Inventory.Count){if(GameModel.IsContraband(m.S.Inventory[i]))Text("!",x+108,802,16,Orange,true);Item(m.S.Inventory[i],x+40,806);Center(m.S.Inventory[i],x,840,126,13,Ink);}else Center("—",x,822,126,18,C("#56716B"));}
  Box(Panel,876,776,376,90);Wrap(m.Message,894,790,339,64,15,Ink);
  Text("WASD / săgeți  ·  Shift  ·  E  ·  F  ·  C  ·  T  ·  K  ·  Esc",28,876,12,Muted);Text(savingFailed?"Salvarea nu a reușit — F5 reîncearcă":"Salvare automată locală  ·  F5 salvează",878,876,12,savingFailed?Orange:Muted);
  if(screen!=""||m.S.Won||m.S.Health<=0)Overlay(m,m.S.Won?"won":m.S.Health<=0?"lost":screen,hasSave);
 }
 void Tile(int x,int y,Color c){Box(c,MapX+x*T,MapY+y*T,T,T);}
 void DrawMap(GameModel m){
  Box(C("#0D1B1C"),MapX-3,MapY-3,GameModel.W*T+6,GameModel.H*T+6);
  for(int y=0;y<GameModel.H;y++)for(int x=0;x<GameModel.W;x++){
   int px=MapX+x*T,py=MapY+y*T,t=m.Tiles[x,y],r=(x*43+y*17)%9;
   if(t==0){Tile(x,y,C(r<4?"#73876A":"#7B8D70"));Box(C("#61755C"),px+4+r,py+7,2,3);Box(C("#91A182"),px+16,py+14+r%4,3,2);}
   if(t==1){Tile(x,y,C((x+y)%2==0?"#CBC3A5":"#C2BA9E"));Line(C("#B2AD93"),px,py+23,px+24,py+23);Line(C("#B2AD93"),px+23,py,px+23,py+24);}
   if(t==4){Tile(x,y,C(r<4?"#ABAF98":"#B2B49D"));Box(C("#969F8A"),px+8,py+17,2,2);}
   if(t==2){Tile(x,y,C("#596B61"));Box(C("#E0D9B8"),px,py,24,15);Box(C("#CAC5AA"),px,py+15,24,4);Line(C("#A6AA93"),px,py+7,px+24,py+7);Line(C("#B2B39B"),px+(y%2==0?8:16),py,px+(y%2==0?8:16),py+7);}
   if(t==3){Tile(x,y,C("#687C65"));for(int a=0;a<24;a+=8){Line(C("#ABB7A1"),px+a,py,px+24,py+24-a);Line(C("#ABB7A1"),px,py+a,px+24-a,py+24);}Box(C("#374E4B"),px+10,py,3,24);Box(C("#C6C9A7"),px+10,py,3,3);}
  }
  // Yard sports markings and simple fixtures, drawn entirely with original primitives.
  using(var p=new Pen(C("#DDD7B4"),2)){g.DrawRectangle(p,MapX+14*T,MapY+10*T,7*T,6*T);g.DrawEllipse(p,MapX+16*T,MapY+12*T,3*T,2*T);}
  Furniture(5,7,"bed");Furniture(10,5,"bed");Furniture(10,7,"bed");Furniture(6,19,"books");Furniture(4,17,"books");Furniture(10,20,"books");
  int sx=MapX+26*T,sy=MapY+9*T;Box(m.MaterialPermit?Green:Orange,sx,sy,3,24);Box(m.MaterialPermit?Green:Orange,sx+21,sy,3,24);Label("DETECTOR",25,10);Furniture(28,15,"table");Label("GHIȘEU",28,14);
  Furniture(24,6,"bench");Furniture(27,6,"bench");Furniture(24,20,"table");Furniture(28,20,"table");Furniture(5,12,"bench");Furniture(9,12,"bench");
  foreach(Crate c in m.S.Crates){int x=MapX+c.X*T,y=MapY+c.Y*T;Box(C("#756549"),x,y+5,24,19);Box(C("#AE8F5F"),x,y+2,24,17);Box(C("#CAB580"),x+3,y+4,18,3);Box(C("#E9DCAF"),x+10,y+11,4,3);if(c.Items.Count>0){Box(Orange,x+19,y-2,5,5);}}
  int hx=MapX+10*T,hy=MapY+21*T;Box(Dark,hx,hy,24,21);Box(m.S.TunnelStage==3?Green:Orange,hx+3,hy+3,18,15);Line(Dark,hx+5,hy+9,hx+19,hy+9,3);Box(Dark,hx-2,hy+23,29,17);Text(m.S.TunnelStage+"/3",hx,hy+24,11,Ink,true);
  Label("CELULA TA",3,2);Label("ATELIER",23,2);Label("BIBLIOTECĂ",3,15);Label("CANTINĂ",23,16);
  Text("APEL",MapX+16*T+2,MapY+11*T,12,C("#4E6257"),true);
  // Cones stop at obstacles; light is informative, not decorative wall penetration.
  foreach(Actor a in m.Guards){double baseAngle=Math.Atan2(a.DY,a.DX);var points=new List<PointF>{new PointF((float)(MapX+a.X*T),(float)(MapY+a.Y*T))};for(int j=0;j<=20;j++){double angle=baseAngle-1.1+j*2.2/20;double d=.1;for(;d<6.5;d+=.15)if(m.Solid((int)(a.X+Math.Cos(angle)*d),(int)(a.Y+Math.Sin(angle)*d)))break;points.Add(new PointF((float)(MapX+(a.X+Math.Cos(angle)*d)*T),(float)(MapY+(a.Y+Math.Sin(angle)*d)*T)));}using(var brush=new SolidBrush(Color.FromArgb(a.Alert>0?65:27,a.Alert>0?Color.OrangeRed:Color.LightGoldenrodYellow)))g.FillPolygon(brush,points.ToArray());}
  for(int i=0;i<m.Inmates.Length;i++){Actor a=m.Inmates[i];Person(a.X,a.Y,i==1?Green:i==4?C("#8BBAC1"):C("#B88753"),false,false,m.S.Minutes);if(i==m.NearbyInmate()){int nx=(int)(MapX+a.X*T)-28,ny=(int)(MapY+a.Y*T)-36;Box(Dark,nx,ny,64,17);Text("F · "+GameModel.NpcNames[i],nx+3,ny+1,11,Ink,true);}}
  Text("12 cr.",MapX+24*T-2,MapY+6*T-18,10,C("#324A3B"),true);
  foreach(Actor a in m.Guards){Person(a.X,a.Y,C("#355D64"),true,false,m.S.Minutes);if(a.Alert>0)Text("!",(float)(MapX+a.X*T-3),(float)(MapY+a.Y*T-34),23,C("#FFBF78"),true);}
  Person(m.S.X,m.S.Y,C("#EBC477"),false,true,m.S.Minutes);
  int fx=MapX+32*T,fy=MapY+12*T;using(var p=new Pen(m.S.FenceOpen?Green:Orange,2))g.DrawRectangle(p,fx-2,fy-2,28,28);Text("›",fx+24,fy-6,29,Orange,true);
  if(m.Routine=="STINGEREA")Box(Color.FromArgb(75,13,29,46),MapX,MapY,GameModel.W*T,GameModel.H*T);
  int promptY=MapY+GameModel.H*T-30;Box(Color.FromArgb(232,20,37,40),MapX,promptY,GameModel.W*T,30);Text(m.Prompt(),MapX+12,promptY+5,14,Ink);
  if(m.Action!=""){int x=(int)(MapX+m.S.X*T)-31,y=(int)(MapY+m.S.Y*T)-43;Box(Dark,x,y,64,6);Box(Orange,x+1,y+1,(float)(62*m.ActionTime/m.ActionTotal),4);}
 }
 void Label(string s,int tx,int ty){int x=MapX+tx*T,y=MapY+ty*T;int w=(int)g.MeasureString(s,F(11,true)).Width+12;Box(Color.FromArgb(240,38,60,54),x,y+4,w,18);Text(s,x+5,y+4,11,Ink,true);}
 void Furniture(int tx,int ty,string type){int x=MapX+tx*T,y=MapY+ty*T;if(type=="bed"){Box(C("#5E706A"),x-3,y+5,29,19);Box(C("#F2E9C9"),x-3,y,29,20);Box(C("#658C8C"),x+5,y+2,19,16);Box(C("#FFFAE0"),x-1,y+2,6,14);}
  else if(type=="books"){Box(C("#705E47"),x,y,24,26);for(int j=0;j<4;j++){Box(j%2==0?C("#9A5545"):C("#5E8989"),x+2+j*5,y+3,4,8);Box(j%2==0?C("#B7A36B"):C("#889672"),x+2+j*5,y+15,4,8);}Box(C("#B19768"),x,y+12,24,2);}
  else{Box(C("#5D6857"),x,y+8,44,17);Box(C("#AA9166"),x-2,y,46,18);Line(C("#CCB480"),x,y+5,x+41,y+5);Line(C("#796946"),x,y+10,x+41,y+10);if(type=="table"){Box(C("#E5DBB5"),x+4,y+3,8,8);Box(C("#D1C29B"),x+26,y+3,8,8);}}}
 void Person(double wx,double wy,Color shirt,bool guard,bool player,double time){int x=(int)(MapX+wx*T)-6,y=(int)(MapY+wy*T)-13;Box(Color.FromArgb(75,20,32,27),x-3,y+17,20,5);int stride=(int)(time*5)%2;Box(C("#303D36"),x+1,y+14,4,6+stride);Box(C("#303D36"),x+8,y+14,4,7-stride);Box(shirt,x,y+6,13,10);Box(C("#D4AE7F"),x-2,y+8,3,7);Box(C("#D4AE7F"),x+12,y+8,3,7);Box(C("#E7C293"),x+2,y-1,9,9);Box(guard?C("#284D58"):C("#655541"),x+1,y-3,11,4);Box(C("#293F39"),x+8,y+3,2,2);if(guard)Box(C("#E3BF73"),x+8,y+8,3,3);if(player){Line(Orange,x-6,y+25,x+18,y+25,2);using(var b=new SolidBrush(Orange))g.FillPolygon(b,new[]{new Point(x+3,y-15),new Point(x+11,y-15),new Point(x+7,y-10)});}}
 void Item(string name,int x,int y){if(name=="lopată"){Line(C("#CA8F57"),x+10,y+2,x+24,y+18,5);Box(C("#B8CCC4"),x+19,y+15,14,12);}else if(name=="bandă"){using(var b=new SolidBrush(C("#B1BCB5")))g.FillEllipse(b,x+7,y+2,24,22);using(var b=new SolidBrush(Panel))g.FillEllipse(b,x+13,y+8,12,10);}else if(name=="biscuiți"){Box(C("#E7BB68"),x+7,y+2,23,23);Box(C("#8C6345"),x+11,y+6,3,3);Box(C("#8C6345"),x+22,y+17,3,3);Box(C("#8C6345"),x+17,y+11,3,3);}else if(name=="sârmă"){using(var p=new Pen(C("#9EBFC0"),3)){g.DrawArc(p,x+5,y+2,22,20,0,310);g.DrawArc(p,x+10,y+7,20,18,0,310);}}else if(name=="pilă"){Line(C("#A7C0BB"),x+7,y+4,x+25,y+18,6);Line(C("#CA8F57"),x+25,y+18,x+32,y+24,7);}else{Line(C("#B8CCC4"),x+8,y+1,x+27,y+23,5);Line(C("#B8CCC4"),x+28,y+1,x+8,y+23,5);Line(Orange,x+18,y+13,x+27,y+23,5);Line(Orange,x+17,y+13,x+8,y+23,5);}}
 void Overlay(GameModel m,string screen,bool hasSave){
  Buttons.Clear();Box(Color.FromArgb(206,9,23,26),0,0,Width,Height);
  const int x=340,y=159,w=600,h=580;Box(C("#102125"),x+8,y+10,w,h);Box(Panel,x,y,w,h);Box(Orange,x,y,5,h);
  Text("BLOCUL 17 / "+(screen=="intro"?"PRIMA ÎNCERCARE":screen=="craft"?"ATELIER IMPROVIZAT":screen=="guide"?"GHIDUL DEȚINUTULUI":screen=="journal"?"ÎNSEMNĂRI":"PLANUL TĂU"),x+36,y+27,13,Orange,true);
  if(screen=="security"){DrawSecurity(m,x,y);
  }else if(screen=="evidence"){DrawEvidence(m,x,y);
  }else if(screen=="storage"&&m.StorageCrate>=0){DrawStorage(m,x,y);
  }else if(screen=="talk"&&m.TalkNpc>=0&&m.TalkNpc<5){DrawSocial(m,x,y);
  }else if(screen=="intro"){
   Text("Învață rutina.",x+35,y+72,42,Ink,true);Text("Găsește breșa.",x+35,y+124,42,Ink,true);
   Wrap("O curte. Două patrule. Șase buzunare. Libertatea începe cu lucrurile pe care ceilalți le ignoră.",x+38,y+205,505,85,21,Ink);
   Wrap("Lucrează, fă favoruri și cumpără materiale. Alege cleștele pentru gardul din est sau lopata pentru tunelul bibliotecii, cu ieșire noaptea.",x+38,y+309,505,102,17,Muted);
   Button("start",hasSave?"Continuă partida  →":"Intră în celulă  →",x+38,y+451,330,51,true);Button("guide","Ghid · F1",x+383,y+451,179,51);
   Text("WASD mișcare  ·  E obiecte  ·  F dialog  ·  C crafting",x+38,y+523,14,Muted);
  }else if(screen=="craft"){
   Text("Alege drumul spre libertate",x+35,y+73,30,Ink,true);
   Text("Fiecare unealtă consumă pilă + sârmă + bandă.",x+38,y+122,16,Muted);
   Box(Dark,x+38,y+166,524,107);Item("clește",x+49,y+191);Text("CLEȘTE · GARDUL DIN EST",x+100,y+178,19,Ink,true);Text("30 intelect · 8 secunde · 30 energie",x+100,y+207,15,Muted);Button("make","1 / Enter · Construiește",x+100,y+236,448,30,true,m.S.Intellect>=30&&new[]{"pilă","sârmă","bandă"}.All(i=>m.S.Inventory.Contains(i)));
   Box(Dark,x+38,y+286,524,107);Item("lopată",x+49,y+311);Text("LOPATĂ · TUNELUL BIBLIOTECII",x+100,y+298,19,Ink,true);Text("40 intelect · 3 etape · 20 energie / etapă",x+100,y+327,15,Muted);Button("make-shovel","2 · Construiește",x+100,y+356,448,30,true,m.S.Intellect>=40&&new[]{"pilă","sârmă","bandă"}.All(i=>m.S.Inventory.Contains(i)));
   Wrap(m.Message,x+38,y+412,515,72,16,Ink);Button("close","Înapoi · Esc",x+38,y+505,524,44);
  }else if(screen=="guide"){
   Text("Lucrurile care contează",x+35,y+72,31,Ink,true);
   string[] lines={"WASD / săgeți     Deplasare       Shift     Alergare","E     Obiecte, studiu, muncă, gard sau tunel","F     Dialog: favoruri, comerț și sfaturi","C     Unelte: 1 clește (30 intelect), 2 lopată (40)","1–6 / click     Folosește obiectul din buzunar","E la recipient · H ascunzătoare · V depune","T Așteaptă ×20 · K Securitate · J Jurnal · F5 Salvare"};for(int i=0;i<lines.Length;i++)Text(lines[i],x+38,y+140+i*34,16,Ink);
   Wrap("Tunel: trapa din sud-estul bibliotecii. Sapă 3 etape de 8 secunde; ieși noaptea, între 22:00 și 07:00. Capturarea astupă tunelul. Atelier: 09:00–12:00. F pentru favoruri și comerț.",x+38,y+396,515,95,16,Muted);Button("close","Am înțeles  →",x+38,y+505,524,44,true);
  }else if(screen=="journal"){
   Text("Datorii și promisiuni.",x+35,y+73,31,Ink,true);Box(Dark,x+38,y+131,524,115);Text("FAVOR ACTIV",x+51,y+143,12,Orange,true);Wrap(m.FavorSummary,x+51,y+169,345,65,17,Ink);Button("abandon","Renunță",x+419,y+180,128,38,false,m.S.ActiveFavor>=0);Text("Muncă azi: "+m.S.WorkDone+"/3  ·  Bani: "+m.S.Money+" cr.",x+38,y+265,15,Green);for(int i=0;i<Math.Min(4,m.Log.Count);i++)Wrap(m.Log[i],x+38,y+307+i*44,515,43,14,i==0?Ink:Muted);Button("close","Înapoi · Esc",x+38,y+505,250,44,true);Button("security","Securitate",x+304,y+505,258,44);
  }else if(screen=="won"||screen=="lost"){
   Text(screen=="won"?(m.S.EscapedTunnel?"DINCOLO DE ZIDURI.":"DINCOLO DE GARD."):"TRANSFER DISCIPLINAR.",x+35,y+105,32,Ink,true);
   Wrap(screen=="won"?"Pentru prima dată, nu mai aștepți următorul apel. Ești liber.":"Capturările ți-au epuizat sănătatea. Harta are încă o breșă. Următorul plan poate fi mai bun.",x+38,y+184,515,116,23,Ink);
   Text("ZIUA "+m.S.Day+"   /   "+m.Clock+"   /   CAPTURĂRI "+m.S.Caught,x+38,y+338,16,Orange,true);Button("restart","O nouă încercare",x+38,y+451,330,51,true);Button("quit","Închide",x+383,y+451,179,51);
  }else if(screen=="confirm"){
   Text("Un plan de la zero?",x+35,y+103,36,Ink,true);Wrap("Partida salvată va fi înlocuită. Începi din nou în celulă, cu materialele refăcute.",x+38,y+198,510,115,23,Ink);Button("restart","Începe din nou",x+38,y+451,330,51,true);Button("close","Păstrează partida",x+383,y+451,179,51);
  }else{
   Text("Planul poate aștepta.",x+35,y+92,36,Ink,true);Text("Jocul este în pauză.",x+38,y+153,19,Muted);
   Button("close","Continuă · Esc",x+38,y+230,524,49,true);Button("save","Salvează partida",x+38,y+294,254,47);Button("guide","Ghid · F1",x+308,y+294,254,47);Button("confirm","Încearcă de la zero",x+38,y+357,524,47);Button("quit","Salvează și închide",x+38,y+451,524,47);
  }
 }
 public void Dispose(){foreach(var f in fonts.Values)f.Dispose();}
}
}
