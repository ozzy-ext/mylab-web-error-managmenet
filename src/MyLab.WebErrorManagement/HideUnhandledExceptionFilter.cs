﻿using System.Buffers;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Formatting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MyLab.WebErrorManagement
{
    class HideUnhandledExceptionFilter : IExceptionFilter
    {
        private readonly ExceptionProcessingOptions _options;

        public const string DefaultMessage = "An error occurred during the operation";

        public string Message { get; set; } = DefaultMessage;

        public HideUnhandledExceptionFilter(IOptions<ExceptionProcessingOptions> options)
        {
            _options = options.Value;
        }

        public void OnException(ExceptionContext context)
        {
            if (!(_options?.HideError ?? true))
                return;

            var dto = new InterlevelErrorDto
            {
                Id = context.Exception.GetId(),
                Message = _options?.HidesMessage ?? Message ?? DefaultMessage
            };

            var res = new JsonResult(dto)
            {
                StatusCode = (int) HttpStatusCode.InternalServerError
            };
            
            context.Result = res;
            context.ExceptionHandled = true;
        }
    }
}