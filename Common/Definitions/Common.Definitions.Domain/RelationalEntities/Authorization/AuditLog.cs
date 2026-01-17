using System.ComponentModel.DataAnnotations.Schema;
using Common.Definitions.Base.Entity;
using Common.Definitions.Base.Enums;

namespace Common.Definitions.Domain.Entities;

public class AuditLog : BaseEntity
{
	public AllModules ModuleType { get; set; }

	[Column(TypeName = "varchar(100)")]
	public string ObjectName { get; set; } //Statistics

	[Column(TypeName = "varchar(100)")]
	public string ActionName { get; set; } //GetStatistics

	public EndpointTypes EndpointType { get; set; } //Command-Query
	public bool IsResponseModelArray { get; set; } //Payload dizi mi obje mi vs
	public bool IsInternal { get; set; } //EventHAndler lar genelde Internal olmalı. Dışardan çağırılmasını istemiyorsak InternalHandler Attribute unu ekliyoruz. Örneğin Ebys deki dışardan çağırılanlarda kaldırılmış.
	public bool IsAuthorize { get; set; } //Jwt vb auth kontrolü yapılacak mı? ApiGateway de Jwt kontrolleri yapılıyor. 
	public bool IsAllowAnonymous { get; set; } // Login de örneğin hiç Jwt si yok. Qr oluşturma kısımlarında yok
	public bool IsEventHandler { get; set; } // Request handler degil, event handler

	[Column(TypeName = "varchar(36)")]
	public string RequestId { get; set; }

	[Column(TypeName = "varchar(35)")]
	public string UserIp { get; set; }

	public int StatusCode { get; set; }
	public bool HasError { get; set; }

	[Column(TypeName = "nvarchar(100)")]
	public string Code { get; set; } // Statistics_GetStatistics_Succedded gibi

	[Column(TypeName = "nvarchar(250)")]
	public string ErrorCode { get; set; } // USER_ID_NOT_VALID gibi

	[Column(TypeName = "nvarchar(1000)")]
	public string ListPageAsJson { get; set; }

	[Column(TypeName = "nvarchar(2000)")]
	public string ListFilterAsJson { get; set; }

	[Column(TypeName = "nvarchar(2000)")]
	public string ListSortingAsJson { get; set; }

	[Column(TypeName = "nvarchar(max)")]
	public string RequestPayloadAsJson { get; set; }

	[Column(TypeName = "nvarchar(max)")]
	public string ResponsePayloadAsJson { get; set; }

	[Column(TypeName = "nvarchar(max)")]
	public string HeadersAsJson { get; set; }

	public int DurationMs { get; set; } // İsteğin ne kadar sürdüğü
	public int TotalDurationMs { get; set; } // İsteğin ne kadar sürdüğü w/PreHandlerPostHandler

	//Relation
	public Guid? UserId { get; set; }
	public User User { get; set; }
}

public enum EndpointTypes
{
	Command = 0,
	Query = 1,
}