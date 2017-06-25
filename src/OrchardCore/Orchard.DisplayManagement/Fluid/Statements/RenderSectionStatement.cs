﻿using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;
using Fluid.Ast;
using Orchard.DisplayManagement.Fluid.Ast;

namespace Orchard.DisplayManagement.Fluid.Statements
{
    public class RenderSectionStatement : Statement
    {
        private readonly FilterArgumentsExpression _arguments;

        public RenderSectionStatement(FilterArgumentsExpression arguments)
        {
            _arguments = arguments;
        }

        public override async Task<Completion> WriteToAsync(TextWriter writer, TextEncoder encoder, TemplateContext context)
        {
            if (context.AmbientValues.TryGetValue("FluidView", out var view) && view is FluidView)
            {
                var arguments = (await _arguments.EvaluateAsync(context)).ToObjectValue() as FilterArguments;

                var name = arguments.HasNamed("name") ? arguments["name"].ToStringValue() : String.Empty;
                var required = arguments.HasNamed("required") ? Convert.ToBoolean(arguments["required"].ToStringValue()) : false;

                await writer.WriteAsync((await (view as FluidView).RenderSectionAsync(name, required)).ToString());
            }
            else
            {
                throw new ParseException("FluidView missing while invoking 'rendersection'.");
            }

            return Completion.Normal;
        }
    }
}