global using System;
global using System.Text;
global using System.Web;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Collections.Generic;
global using System.Globalization;
global using Microsoft.AspNetCore.Http;
global using Microsoft.EntityFrameworkCore;
global using static System.Linq.Queryable;
global using static System.Linq.Enumerable;

global using FluentValidation;
global using FluentAssertions;

global using Arfware.ArfBlocks.Core;
global using Arfware.ArfBlocks.Core.Abstractions;
global using Arfware.ArfBlocks.Core.RequestResults;
global using Arfware.ArfBlocks.Core.Exceptions;
global using Arfware.ArfBlocks.Test;
global using Arfware.ArfBlocks.Test.Abstractions;
global using Arfware.ArfBlocks.Core.Attributes;
global using Arfware.ArfBlocks.Core.Models;
global using Arfware.ArfBlocks.Core.Contexts;

global using Common.Definitions.Domain.Entities;
global using Common.Definitions.Domain.Models;
global using Common.Definitions.Infrastructure.Services;
global using Common.Definitions.Infrastructure.RelationalDB;
global using Common.Services.Environment;
global using Common.Services.Auth.CurrentUser;
global using Common.Services.Auth.Authorization;
global using Common.Services.ErrorCodeGenerator;
global using Common.Definitions.Base.Enums;

global using LivestockTrading.Infrastructure.RelationalDB;
global using LivestockTrading.Domain.Entities;
global using LivestockTrading.Domain.Events;
global using Common.Services.Messaging;
global using ArfFipaso.Filter.Models;
global using ArfFipaso.Pagination.Models;
global using ArfFipaso.Sorting.Models;
global using ArfFipaso.Sorting.Extensions;
global using ArfFipaso.Filter.Extensions;
global using ArfFipaso.Pagination.Extensions;
global using LivestockTrading.Domain.Errors;
global using LivestockTrading.Application.Extensions;
