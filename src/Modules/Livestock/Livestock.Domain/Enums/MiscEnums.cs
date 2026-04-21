namespace Livestock.Domain.Enums;

public enum NotificationType { OrderPlaced = 0, OrderShipped = 1, OrderDelivered = 2, OrderCancelled = 3, PaymentReceived = 4, PaymentFailed = 5, NewMessage = 6, ProductBackInStock = 7, PriceDropAlert = 8, NewReview = 9, SellerVerified = 10, ProductApproved = 11, ProductRejected = 12, ProductPendingApproval = 13, SellerPendingVerification = 14, TransporterPendingVerification = 15, System = 99 }
public enum BannerPosition { Homepage = 0, CategoryPage = 1, ProductPage = 2, SearchResults = 3, Sidebar = 4 }
public enum ChemicalType { Fertilizer = 0, Pesticide = 1, Herbicide = 2, Fungicide = 3, Insecticide = 4, Rodenticide = 5, PlantGrowthRegulator = 6, SoilConditioner = 7, Disinfectant = 8, Other = 99 }
public enum ToxicityLevel { NonToxic = 0, SlightlyToxic = 1, ModeratelyToxic = 2, HighlyToxic = 3, ExtremelyToxic = 4 }
public enum FeedType { CompleteFeed = 0, Supplement = 1, Concentrate = 2, Forage = 3, Hay = 4, Silage = 5, Grain = 6, MineralBlock = 7, VitaminSupplement = 8, ProteinSupplement = 9, Premix = 10, MilkReplacer = 11, Other = 99 }
public enum FeedForm { Pellets = 0, Crumbles = 1, Mash = 2, Granules = 3, Powder = 4, Liquid = 5, Block = 6, Cubes = 7, Bales = 8, Other = 99 }
public enum MachineryType { Tractor = 0, Harvester = 1, Plow = 2, Seeder = 3, Sprayer = 4, Cultivator = 5, Baler = 6, Mower = 7, IrrigationSystem = 8, Generator = 9, Pump = 10, Trailer = 11, HandTool = 12, PowerTool = 13, ProcessingEquipment = 14, StorageEquipment = 15, Other = 99 }
public enum SeedType { VegetableSeeds = 0, FruitSeeds = 1, GrainSeeds = 2, HerbSeeds = 3, FlowerSeeds = 4, GrassSeed = 5, TreeSeedlings = 6, Transplants = 7, Bulbs = 8, Tubers = 9, CoverCropSeeds = 10, Other = 99 }
public enum VeterinaryProductType { Antibiotic = 0, Antiparasitic = 1, Vaccine = 2, AntiInflammatory = 3, Analgesic = 4, Vitamin = 5, Hormone = 6, Anesthetic = 7, Antiseptic = 8, Bandage = 9, MedicalEquipment = 10, DiagnosticKit = 11, Disinfectant = 12, WoundCare = 13, Other = 99 }
public enum AdministrationRoute { Oral = 0, Injection = 1, Topical = 2, Intravenous = 3, Intramuscular = 4, Subcutaneous = 5, Inhalation = 6, Ophthalmic = 7, Otic = 8, Rectal = 9, Other = 99 }
