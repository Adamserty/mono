// 
// DeleteActionTests.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using NUnit.Framework;

namespace Mono.TextEditor.Tests.Actions
{
	[TestFixture()]
	public class DeleteActionTests : TextEditorTestBase
	{
		[Test()]
		public void TestBackspace ()
		{
			TextEditorData data = CaretMoveActionTests.Create (@"1234$567890");
			Assert.AreEqual (new DocumentLocation (1, 5), data.Caret.Location);
			DeleteActions.Backspace (data);
			Assert.AreEqual (new DocumentLocation (1, 4), data.Caret.Location);
			Assert.AreEqual ("123567890", data.Document.Text);
		}
		
		[Test()]
		public void TestBackspaceCase1 ()
		{
			TextEditorData data = CaretMoveActionTests.Create (@"$1234567890");
			DeleteActions.Backspace (data);
			Assert.AreEqual (new DocumentLocation (1, 1), data.Caret.Location);
			Assert.AreEqual ("1234567890", data.Document.Text);
		}
		
		[Test()]
		public void TestDelete ()
		{
			TextEditorData data = CaretMoveActionTests.Create (@"1234$567890");
			Assert.AreEqual (new DocumentLocation (1, 5), data.Caret.Location);
			DeleteActions.Delete (data);
			Assert.AreEqual (new DocumentLocation (1, 5), data.Caret.Location);
			Assert.AreEqual ("123467890", data.Document.Text);
		}
		
		[Test()]
		public void TestBackspaceDeleteCase1 ()
		{
			TextEditorData data = CaretMoveActionTests.Create (@"1234567890$");
			DeleteActions.Delete (data);
			Assert.AreEqual (new DocumentLocation (1, 11), data.Caret.Location);
			Assert.AreEqual ("1234567890", data.Document.Text);
		}
		
		[Test()]
		public void TestDeleteCaretLine ()
		{
			TextEditorData data = CaretMoveActionTests.Create (@"1234567890
1234$67890
1234567890");
			DeleteActions.CaretLine (data);
			Assert.AreEqual (@"1234567890
1234567890", data.Document.Text);
		}
		
		[Test()]
		public void TestDeleteCaretLineToEnd ()
		{
			TextEditorData data = CaretMoveActionTests.Create (@"1234567890
1234$67890
1234567890");
			DeleteActions.CaretLineToEnd (data);
			Assert.AreEqual (@"1234567890
1234
1234567890", data.Document.Text);
		}

		[Test()]
		public void TestDeletePreviousWord ()
		{
			var data = Create (@"      word1 word2 word3$");
			DeleteActions.PreviousWord (data);
			Check (data, @"      word1 word2 $");
			DeleteActions.PreviousWord (data);
			Check (data, @"      word1 $");
			DeleteActions.PreviousWord (data);
			Check (data, @"      $");
		}
		
		[Test()]
		public void TestDeletePreviousSubword ()
		{
			var data = Create (@"      SomeLongWord$");
			DeleteActions.PreviousSubword (data);
			Check (data, @"      SomeLong$");
			DeleteActions.PreviousSubword (data);
			Check (data, @"      Some$");
			DeleteActions.PreviousSubword (data);
			Check (data, @"      $");
		}

		[Test()]
		public void TestDeleteNextWord ()
		{
			var data = Create (@"      $word1 word2 word3");
			DeleteActions.NextWord (data);
			Check (data, @"      $ word2 word3");
			DeleteActions.NextWord (data);
			Check (data, @"      $ word3");
			DeleteActions.NextWord (data);
			Check (data, @"      $");
		}

		[Test()]
		public void TestDeleteNextSubword ()
		{
			var data = Create (@"      $SomeLongWord");
			DeleteActions.NextSubword (data);
			Check (data, @"      $LongWord");
			DeleteActions.NextSubword (data);
			Check (data, @"      $Word");
			DeleteActions.NextSubword (data);
			Check (data, @"      $");
		}
	}
}
