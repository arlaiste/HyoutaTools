﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTVA {
	public class T8BTVA {
		public T8BTVA( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading T8BTVA failed!" );
				}
			}
		}

		public T8BTVA( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading T8BTVA failed!" );
			}
		}

		public List<T8BTVABlock> Blocks;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint blockCount = stream.ReadUInt32().SwapEndian();
			uint refStringStart = stream.ReadUInt32().SwapEndian();

			Blocks = new List<T8BTVABlock>( (int)blockCount );
			for ( uint i = 0; i < blockCount; ++i ) {
				Blocks.Add( new T8BTVABlock( stream, refStringStart ) );
			}

			return true;
		}
	}

	public class T8BTVABlock {
		public uint Size;
		public uint IndexInArray;
		public uint IndexInGame;
		public uint IdentifierLocation;
		public string Identifier;
		public uint Unknown5;
		public uint Unknown6;
		public uint Unknown7;
		public uint Unknown8;
		public uint Unknown9;
		public uint Unknown10;
		public uint Unknown11;
		public uint Unknown12;
		public uint Unknown13;
		public uint EntryCount;

		public List<T8BTVAEntry> Entries;

		public T8BTVABlock( System.IO.Stream stream, uint refStringStart ) {
			uint size = stream.PeekUInt32().SwapEndian();
			if ( size % 4 != 0 ) { throw new Exception( "T8BTVABlock size must be divisible by 4!" ); }
			if ( size / 4 < 14 ) { throw new Exception( "T8BTVABlock must contain at least 14 ints!" ); }
			Size = stream.ReadUInt32().SwapEndian();
			IndexInArray = stream.ReadUInt32().SwapEndian();
			IndexInGame = stream.ReadUInt32().SwapEndian();
			IdentifierLocation = stream.ReadUInt32().SwapEndian();
			Unknown5 = stream.ReadUInt32().SwapEndian();
			Unknown6 = stream.ReadUInt32().SwapEndian();
			Unknown7 = stream.ReadUInt32().SwapEndian();
			Unknown8 = stream.ReadUInt32().SwapEndian();
			Unknown9 = stream.ReadUInt32().SwapEndian();
			Unknown10 = stream.ReadUInt32().SwapEndian();
			Unknown11 = stream.ReadUInt32().SwapEndian();
			Unknown12 = stream.ReadUInt32().SwapEndian();
			Unknown13 = stream.ReadUInt32().SwapEndian();
			EntryCount = stream.ReadUInt32().SwapEndian();

			Identifier = stream.ReadAsciiNulltermFromLocationAndReset( IdentifierLocation + refStringStart );

			Entries = new List<T8BTVAEntry>( (int)EntryCount );
			for ( uint i = 0; i < EntryCount; ++i ) {
				Entries.Add( new T8BTVAEntry( stream, refStringStart ) );
			}
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append( Identifier );
			sb.Append( ": [ " );
			for ( int i = 0; i < Entries.Count; ++i ) {
				sb.Append( Entries[i].ToString() );
				sb.Append( ", " );
			}
			sb.Append( "]" );
			return sb.ToString();
		}
	}

	public class T8BTVAEntry {
		public uint[] Data;
		public string RefString;
		public uint ScenarioStart;
		public uint ScenarioEnd;
		public uint CharacterBitmask;

		public T8BTVAEntry( System.IO.Stream stream, uint refStringStart ) {
			Data = new uint[33];
			for ( int i = 0; i < 33; ++i ) {
				Data[i] = stream.ReadUInt32().SwapEndian();
			}

			RefString = stream.ReadAsciiNulltermFromLocationAndReset( refStringStart + Data[22] );
			ScenarioStart = Data[2];
			ScenarioEnd = Data[3];
			CharacterBitmask = Data[5];
		}

		public override string ToString() {
			return RefString;
		}
	}
}
