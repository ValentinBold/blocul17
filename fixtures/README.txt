save-v1.xml este o probă sintetică produsă de codul GameModel.cs din
livrarea Blocul17_Windows_v0.1.0: new GameModel(), bandă în inventar,
intelect 40, apoi Save(). Nu este o salvare personală a utilizatorului.
Tests.cs verifică migrarea acestui format real către versiunea 3.

save-v2.xml este produs de executabilul 0.2.0 cu new GameModel(),
Money=27, ActiveFavor=0, WorkDone=2, apoi Save(). Nu este o salvare personală.
TunnelTests.cs verifică păstrarea economiei la migrarea în versiunea 3.

save-v3.xml a fost produs de executabilul 0.3.0: new GameModel(),
Money=19, TunnelStage=2, Inventory.Add("lopată"), Save(). Este sintetic.
StorageTests.cs verifică migrarea către v4 și persistența ascunzătorii.
