#if UNITY_EDITOR

// +----------------------------------------------------------------------+
// |	CompilationStats.cs by Наташа
// |
// |	Project.Editor
// |	(c)2024-2026 Damn Cat Entertainment
// +----------------------------------------------------------------------+
// 
// +----------------------------------------------------------------------+
// |	MIT LICENSE
// +----------------------------------------------------------------------+
// |	THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// |	EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// |	MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// |	NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
// |	BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// |	ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// |	CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// |	SOFTWARE.
// +----------------------------------------------------------------------+

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace System.Runtime.CompilerServices {
	internal class IsExternalInit;
}

namespace Project.Editor {
	[InitializeOnLoad]
	internal static class CompilationStats {
		private static readonly Dictionary<object, Info> _contexts = new();

		private record Info(Stopwatch watch);

		static CompilationStats() {
			_contexts.Clear();

			CompilationPipeline.compilationStarted  -= Begin;
			CompilationPipeline.compilationStarted  += Begin;
			CompilationPipeline.compilationFinished -= End;
			CompilationPipeline.compilationFinished += End;
		}

		private static void Begin(object ctx) {
			Debug.Log(string.Format(new CultureInfo("en-US", false), "[Compiler] >> Start to compile. // {0:yyyy MMMM dd - HH:mm:ss.fff}", DateTime.Now));

			_contexts[ctx] = new Info(Stopwatch.StartNew());
		}

		private static void End(object ctx) {
			if(_contexts.TryGetValue(ctx, out Info info)) {
				Stopwatch sw = info.watch;
				sw.Stop();
				double ms = sw.Elapsed.TotalMilliseconds;

				Debug.LogFormat("[Compiler] >> Finished. (DURATION: <color=#{0}>{1:F2}</color> ms)", ColorUtility.ToHtmlStringRGB(Color.Lerp(Color.mediumSpringGreen, Color.crimson, (float)ms / 500f)), ms);
				_contexts.Remove(ctx);
				return;
			}

			Debug.LogFormat("[Compiler] >> Finished. <color=#{0}>(NO CONTEXT FOUND)</color>", ColorUtility.ToHtmlStringRGB(Color.crimson));
			_contexts.Clear();
		}
	}
#endif
}
