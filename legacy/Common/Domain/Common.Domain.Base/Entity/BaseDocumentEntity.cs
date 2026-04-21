using System.ComponentModel.DataAnnotations.Schema;
using Common.Definitions.Base.Logics;

namespace Common.Definitions.Base.Entity;

public class BaseDocumentEntity : BaseEntity
{
	public string ReferenceNo { get; set; }

	// Get only
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int RowNumber { get; set; }

	[NotMapped]
	public string RefName { get; set; }

	public void SetReferenceNo()
	{
		var referenceNumber = ReferenceNoLogics.GetAdjustedReferenceNo(this.RowNumber);
		this.ReferenceNo = $"{this.RefName}-{referenceNumber}";
	}

	public void SetCustomReferenceNo(string refName, int rowNumber)
	{
		var referenceNumber = ReferenceNoLogics.GetAdjustedReferenceNo(rowNumber);
		this.ReferenceNo = $"{refName}-{referenceNumber}";
	}
}