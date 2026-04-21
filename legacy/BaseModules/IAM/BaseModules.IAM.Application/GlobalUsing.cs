global using System;
global using System.Text;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using System.Threading;
global using System.Globalization;
global using Microsoft.AspNetCore.Http;
global using Microsoft.EntityFrameworkCore;
global using static System.Linq.Queryable;
global using static System.Linq.Enumerable;

//global using MongoDB.Driver;
global using FluentValidation;
global using FluentAssertions;

global using Arfware.ArfBlocks.Core;
global using Arfware.ArfBlocks.Core.Abstractions;
global using Arfware.ArfBlocks.Core.RequestResults;
global using Arfware.ArfBlocks.Core.Exceptions;
global using ArfFipaso.Filter.Extensions;
global using ArfFipaso.Filter.Models;
global using ArfFipaso.Pagination.Extensions;
global using ArfFipaso.Pagination.Models;
global using ArfFipaso.Sorting.Extensions;
global using ArfFipaso.Sorting.Models;
global using Arfware.ArfBlocks.Test;
global using Arfware.ArfBlocks.Test.Abstractions;
global using Arfware.ArfBlocks.Core.Attributes;
global using Arfware.ArfBlocks.Core.Models;
global using Arfware.ArfBlocks.Core.Contexts;


global using Common.Definitions.Domain.Entities;
global using Common.Definitions.Infrastructure.RelationalDB;
global using Common.Services.Auth.CurrentUser;
//global using Common.Connectors.File;
//global using Common.Connectors.Mail;
//global using Common.Connectors.Sms;
global using Common.Services.Auth.JsonWebToken;
global using Common.Services.Environment;
global using Common.Services.Auth.Authorization;
//global using Common.Connectors.File.Models;
//global using Common.Connectors.Mail.Models;
//global using Common.Connectors.Sms.Models;
global using Common.Services.ErrorCodeGenerator;
//global using Common.Definitions.Infrastructure.Services.TestServices;
global using Common.Definitions.Domain.Models;
//global using Common.Contracts.Queue.Models;
global using Common.Definitions.Base.Enums;

//global using BaseModules.IAM.Domain.Errors;
global using BaseModules.IAM.Infrastructure.RelationalDB;
//global using BaseModules.IAM.Domain.Entities;
//global using BaseModules.IAM.Domain.Models;
global using BaseModules.IAM.Infrastructure.Services;

global using Common.Definitions.Domain.Errors;
global using BaseModules.IAM.Infrastructure.Security;
global using BaseModules.IAM.Domain.Entities;