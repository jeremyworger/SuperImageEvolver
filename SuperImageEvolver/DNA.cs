﻿using System;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Text;


namespace SuperImageEvolver {
    public class DNA : ICloneable {
        public DNA() { }

        public DNA( DNA other ) {
            Shapes = new Shape[other.Shapes.Length];
            for( int i = 0; i < Shapes.Length; i++ ) {
                Shapes[i] = new Shape( other.Shapes[i] );
            }
            Divergence = other.Divergence;
        }

        public object Clone() {
            return new DNA( this );
        }
        public MutationType LastMutation;

        public Shape[] Shapes;
        public double Divergence;

        public class Shape : ICloneable {
            public Shape() { }
            public Shape( Shape other ) {
                Color = other.Color;
                Points = (Point[])other.Points.Clone();
            }

            public Color Color;
            public Point[] Points;
            public Shape PreviousState;
            public bool Changed;

            public object Clone() {
                return new Shape( this );
            }

            public void Serialize( Stream stream ) {
                BinaryWriter writer = new BinaryWriter( stream );
                writer.Write( Color.ToArgb() );
                for( int p = 0; p < Points.Length; p++ ) {
                    writer.Write( (short)Points[p].X );
                    writer.Write( (short)Points[p].Y );
                }
            }

            public XElement SerializeSVG( XNamespace xmlns ) {
                XElement el = new XElement( xmlns + "polygon" );
                StringBuilder sb = new StringBuilder();
                foreach( Point point in Points ) {
                    sb.AppendFormat( "{0} {1} ", point.X, point.Y );
                }
                el.Add( new XAttribute( "points", sb.ToString() ) );
                el.Add( new XAttribute( "fill", String.Format( "rgb({0},{1},{2})", Color.R, Color.G, Color.B ) ) );
                el.Add( new XAttribute( "opacity", Color.A/255f ) );
                return el;
            }
        }

        public void Serialize( Stream stream ) {
            for( int i = 0; i < Shapes.Length; i++ ) {
                Shapes[i].Serialize( stream );
            }
        }
    }


    public class Mutation {
        public Mutation( DNA _previousDNA, DNA _newDNA ) {
            PreviousDNA = _previousDNA;
            NewDNA = _newDNA;
            Timestamp = DateTime.UtcNow;
        }

        public DNA PreviousDNA;
        public DNA NewDNA;

        public DateTime Timestamp;

        public double DivergenceDelta {
            get {
                return PreviousDNA.Divergence - NewDNA.Divergence;
            }
        }
    }
}