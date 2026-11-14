// +----------------------------------------------------------------------+
// |	IO.cs by L4ncE
// |
// |	Interlayer Zone.com.team_pawouch.core.io
// |	(c)2024-2026 Team Pawouch
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

#nullable enable
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace com.team_pawouch.core.io.api;

public static class IO {
	private static readonly object _lock       = new();
	private static readonly string _io_evt_log = Path.Combine(Application.persistentDataPath, "io.evt.log").Replace("\\", "/");
	private static readonly string _io_log     = Path.Combine(Application.persistentDataPath, "io.log").Replace("\\", "/");
	private static          string _time => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

	public static IInput? In {
		get => _in;
		set {
			if(value == null) {
				exception(new ArgumentNullException(nameof(value)));
				return;
			}

			if(_in == value) return;

			_in = value;
			_on_input_changed?.Invoke(value);

			_ = appendln(_io_evt_log, "\n{0} IO.on_input_changed({1})", exception, _time, value).ConfigureAwait(false);
		}
	}

	public static IOutput? Out {
		get => _out;
		set {
			if(value == null) {
				exception(new ArgumentNullException(nameof(value)));
				return;
			}

			if(_out == value) return;

			_out = value;
			_on_output_changed?.Invoke(value);

			_ = appendln(_io_evt_log, "\n{0} IO.on_output_changed({1})", exception, _time, value).ConfigureAwait(false);
		}
	}

	public static event Action<IInput>? on_input_changed {
		add {
			if(value == null) {
				exception(new ArgumentNullException(nameof(value)));
				return;
			}

			lock(_lock) {
				_on_input_changed -= value;
				_on_input_changed += value;
			}

			_ = appendln(_io_evt_log, "\n{0} IO.on_input_changed - {1}", exception, _time, value.Method.Name).ConfigureAwait(false);
			_ = appendln(_io_evt_log, "\n{0} IO.on_input_changed + {1}", exception, _time, value.Method.Name).ConfigureAwait(false);
		}
		remove {
			if(value == null) {
				exception(new ArgumentNullException(nameof(value)));
				return;
			}

			lock(_lock) _on_input_changed -= value;

			_ = appendln(_io_evt_log, "\n{0} IO.on_input_changed - {1}", exception, _time, value.Method.Name).ConfigureAwait(false);
		}
	}

	private static event Action<IInput>? _on_input_changed;

	public static event Action<IOutput>? on_output_changed {
		add {
			if(value == null) {
				exception(new ArgumentNullException(nameof(value)));
				return;
			}

			lock(_lock) {
				_on_output_changed -= value;
				_on_output_changed += value;
			}

			_ = appendln(_io_evt_log, "\n{0} IO.on_output_changed - {1}", exception, _time, value.Method.Name).ConfigureAwait(false);
			_ = appendln(_io_evt_log, "\n{0} IO.on_output_changed + {1}", exception, _time, value.Method.Name).ConfigureAwait(false);
		}
		remove {
			if(value == null) {
				exception(new ArgumentNullException(nameof(value)));
				return;
			}

			lock(_lock) _on_output_changed -= value;

			_ = appendln(_io_evt_log, "\n{0} IO.on_output_changed - {1}", exception, _time, value.Method.Name).ConfigureAwait(false);
		}
	}

	private static event Action<IOutput>? _on_output_changed;

	private static          IInput?       _in;
	private static          IOutput?      _out;
	private static readonly SemaphoreSlim _semaphore_slim = new(1, 1);

	public static async Task printf(string format, params object[] args) {
		string str = IO.format(format, args);

		Out?.print(str);

		await appendln(_io_log, "\n{0} {1}", exception, _time, str).ConfigureAwait(false);
	}

	public static async Task println_f(string format, params object[] args) {
		string str = IO.format(format, args);

		Out?.println(str);
		await appendln(_io_log, "\n{0} {1}", exception, _time, str).ConfigureAwait(false);
	}

	public static void exception<t>(t ex) where t : Exception {
		try {
			Out?.println(ex.ToString());
			lock(_lock) {
				if(!File.Exists(_io_log)) File.WriteAllText(_io_log, $"# IO LOG // {_time}", Encoding.UTF8);
				File.AppendAllText(_io_log, ex.ToString(), Encoding.UTF8);
			}
		}
		catch(Exception ex2) {
			Environment.FailFast(ex2.ToString());
		}
	}

	public static string format(string format, object[] args) {
		string str;
		try {
			str = string.Format(format, args);
		}
		catch(Exception ex) {
			str = ex.ToString();
		}

		return str;
	}

	private static async Task<bool> appendln(string file, string format, Action<Exception>? on_exception, params object[] args) {
		try {
			string ln = IO.format(format, args);

			lock(_lock) {
				if(!File.Exists(file)) File.WriteAllText(file, $"# IO LOG // {_time}", Encoding.UTF8);
			}

			await _semaphore_slim.WaitAsync();
			await File.AppendAllTextAsync(file, ln + '\n', Encoding.UTF8).ConfigureAwait(false);
			return true;
		}
		catch(Exception ex) {
			on_exception?.Invoke(ex);
			return false;
		}
		finally {
			_semaphore_slim.Release();
		}
	}
}

// END FILE
