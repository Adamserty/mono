//
// Copyright (c) Microsoft Corp. (https://www.microsoft.com)
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
using System.Reflection.Emit;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding;
using Mono.Addins;

namespace MonoDevelop.TextEditor
{
	class CommandMappingExtensionNode : TypeExtensionNode
	{
		[NodeAttribute ("argsType", Required = true, Description = "The fully qualified type of the EditorCommandArgs subclass")]
		public string ArgsType { get; private set; }

		MappedEditorCommand mappedCommand;

		public MappedEditorCommand GetMappedCommand ()
		{
			return mappedCommand ?? (mappedCommand = CreateMappedCommand ());
			throw new NotImplementedException ();
		}

		MappedEditorCommand CreateMappedCommand ()
		{
			var type = Addin.GetType (ArgsType, true);
			var factory = CreateArgsFactory (type);

			var mapType = typeof (MappedEditorCommand<>).MakeGenericType (type);
			return (MappedEditorCommand) Activator.CreateInstance (mapType, factory);
		}

		class MappedEditorCommand<T> : MappedEditorCommand
			where T : EditorCommandArgs
		{
			Func<ITextView, ITextBuffer, T> factory;

			public MappedEditorCommand (Func<ITextView, ITextBuffer, T> factory)
			{
				this.factory = factory;
			}

			public override void Execute (IEditorCommandHandlerService service, Action nextCommandHandler)
			{
				service.Execute (factory, nextCommandHandler);
			}

			public override CommandState GetCommandState (IEditorCommandHandlerService service, Func<CommandState> nextCommandHandler)
			{
				return service.GetCommandState (factory, nextCommandHandler);
			}
		}

		Delegate CreateArgsFactory (Type type)
		{
			var constructorArgTypes = new Type[] { typeof (ITextView), typeof (ITextBuffer) };
			var ctor = type.GetConstructor (constructorArgTypes);

			var method = new DynamicMethod ($"Create{type.Name}", type, constructorArgTypes);
			var il = method.GetILGenerator ();
			il.Emit (OpCodes.Ldarg_0);
			il.Emit (OpCodes.Ldarg_1);
			il.Emit (OpCodes.Newobj, ctor);
			il.Emit (OpCodes.Ret);

			return method.CreateDelegate (typeof (Func<,,>).MakeGenericType (typeof (ITextView), typeof (ITextBuffer), type));
		}
	}
}