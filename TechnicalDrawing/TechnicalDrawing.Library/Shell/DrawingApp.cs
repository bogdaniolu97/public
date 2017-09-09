﻿using System;
using System.Linq;
using TechnicalDrawing.Library.Contracts;
using TechnicalDrawing.Library.Core;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Library.Shell
{
    public class DrawingApp
    {
        public DrawingApp(FileSystem fs, Parser parser, Projector projector, Canvas canvas)
        {
            this.fs = fs;
            this.parser = parser;
            this.projector = projector;
            this.canvas = canvas;
        }

        public void Load(string filename)
        {
            var parsedLines = fs
                .ReadLines(filename)
                .Select(parser.Parse)
                .ToList();
            var commands = from quadrant in Enum.GetValues(typeof(Plane)).Cast<Plane>()
                           from parsedLine in parsedLines
                           select projector.Project(quadrant, parsedLine);

            foreach (var command in commands)
                canvas.Execute(command);
        }

        //

        private readonly FileSystem fs;
        private readonly Parser parser;
        private readonly Projector projector;
        private readonly Canvas canvas;
    }
}