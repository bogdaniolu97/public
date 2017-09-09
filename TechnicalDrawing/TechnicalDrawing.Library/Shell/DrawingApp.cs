﻿using System;
using System.Linq;
using TechnicalDrawing.Library.Contracts;
using TechnicalDrawing.Library.Core;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Library.Shell
{
    public class DrawingApp
    {
        public DrawingApp(FileSystem fs, Parser parser, Projector projector, Func<Canvas> canvasFactory)
        {
            this.fs = fs;
            this.parser = parser;
            this.projector = projector;
            this.canvasFactory = canvasFactory;
        }

        public void OpenFile(Func<string> getFilename)
        {
            var filename = getFilename();
            if (filename == null)
                return;

            var parsedCommands = fs
                .ReadLines(filename)
                .Select(parser.Parse)
                .ToList();
            var commands = from plane in Enum.GetValues(typeof(Plane)).Cast<Plane>()
                           from parsedCommand in parsedCommands
                           select CreateCommand(plane, parsedCommand);

            var canvas = canvasFactory.Invoke();
            foreach (var command in commands)
                canvas.Execute(command);
        }

        //

        private readonly FileSystem fs;
        private readonly Parser parser;
        private readonly Projector projector;
        private readonly Func<Canvas> canvasFactory;

        /// <summary>Projects a command to the given plane.</summary>
        /// <param name="plane">The plane.</param>
        /// <param name="parsedCommand">The parsed command.</param>
        /// <returns>The command with its coordinates projected to the given plane as <see cref="Point2D"/>s.</returns>
        private ProjectedCommand CreateCommand(Plane plane, ParsedCommand parsedCommand) =>
            new ProjectedCommand(plane, parsedCommand.Name, projector.Project(plane, parsedCommand.Args));
    }
}