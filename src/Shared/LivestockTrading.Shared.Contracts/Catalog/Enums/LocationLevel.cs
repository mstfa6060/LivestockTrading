namespace Shared.Contracts.Catalog;

/// <summary>5-level hiyerarşi (3e madde 1). TR: Country→Region→State(İl)→District(İlçe)→Neighborhood(Mahalle/Köy).</summary>
public enum LocationLevel { Country = 1, Region = 2, State = 3, District = 4, Neighborhood = 5 }
