global using System;
global using System.Text;
global using System.Collections.Generic;
global using System.Threading;
global using System.Linq;
global using System.Threading.Tasks;
global using System.Globalization;
global using Microsoft.AspNetCore.Http;
global using Microsoft.EntityFrameworkCore;

global using MongoDB.Driver;
global using FluentValidation;

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
global using Arfware.ArfBlocks.Core.Attributes;
global using Arfware.ArfBlocks.Test.Abstractions;
global using Arfware.ArfBlocks.Core.Models;
global using Arfware.ArfBlocks.Core.Contexts;


global using Common.Services.Auth.CurrentUser;
global using Common.Services.Auth.JsonWebToken;
global using Common.Services.Environment;
global using Common.Definitions.Infrastructure.RelationalDB;
global using Common.Services.ErrorCodeGenerator;
global using Common.Contracts.Event.Models;
global using Common.Contracts.Queue.Models;
global using Common.Services.Auth.Authorization;
global using Common.Definitions.Base.Enums;

global using Common.Definitions.Domain.NonRelational.Errors;
global using Common.Services.FileOperations;
global using Common.Services.FileOperations.FileStorage;
global using BaseModules.FileProvider.Infrastructure.Services;
global using Common.Definitions.Domain.NonRelational.Entities;
global using Common.Definitions.Infrastructure.DocumentDB;
