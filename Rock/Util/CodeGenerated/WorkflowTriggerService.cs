//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rock.CodeGeneration project
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
//
// THIS WORK IS LICENSED UNDER A CREATIVE COMMONS ATTRIBUTION-NONCOMMERCIAL-
// SHAREALIKE 3.0 UNPORTED LICENSE:
// http://creativecommons.org/licenses/by-nc-sa/3.0/
//

using System;
using System.Linq;

using Rock.Data;

namespace Rock.Util
{
    /// <summary>
    /// WorkflowTrigger Service class
    /// </summary>
    public partial class WorkflowTriggerService : Service<WorkflowTrigger, WorkflowTriggerDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowTriggerService"/> class
        /// </summary>
        public WorkflowTriggerService()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowTriggerService"/> class
        /// </summary>
        public WorkflowTriggerService(IRepository<WorkflowTrigger> repository) : base(repository)
        {
        }

        /// <summary>
        /// Creates a new model
        /// </summary>
        public override WorkflowTrigger CreateNew()
        {
            return new WorkflowTrigger();
        }

        /// <summary>
        /// Query DTO objects
        /// </summary>
        /// <returns>A queryable list of DTO objects</returns>
        public override IQueryable<WorkflowTriggerDto> QueryableDto( )
        {
            return QueryableDto( this.Queryable() );
        }

        /// <summary>
        /// Query DTO objects
        /// </summary>
        /// <returns>A queryable list of DTO objects</returns>
        public IQueryable<WorkflowTriggerDto> QueryableDto( IQueryable<WorkflowTrigger> items )
        {
            return items.Select( m => new WorkflowTriggerDto()
                {
                    IsSystem = m.IsSystem,
                    EntityTypeId = m.EntityTypeId,
                    EntityTypeQualifierColumn = m.EntityTypeQualifierColumn,
                    EntityTypeQualifierValue = m.EntityTypeQualifierValue,
                    WorkflowTypeId = m.WorkflowTypeId,
                    WorkflowTriggerType = m.WorkflowTriggerType,
                    WorkflowName = m.WorkflowName,
                    Id = m.Id,
                    Guid = m.Guid,
                });
        }
    }
}