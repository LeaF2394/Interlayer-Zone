// +----------------------------------------------------------------------+
// |	app_config.cs by L4ncE
// |
// |	Interlayer Zone.com.team_pawouch.core.applications
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
using com.team_pawouch.core.applications.api;
using Newtonsoft.Json;

namespace com.team_pawouch.core.applications;

internal readonly record struct app_config() : IAppConfig {
	internal static IAppConfig? load_from_file(string file_path) {
		if(!File.Exists(file_path)) return null;

		string      json   = File.ReadAllText(file_path);
		IAppConfig? config = JsonConvert.DeserializeObject<app_config>(json);

		return config;
	}

	internal void write_to_file(string file_path) {
		try {
			string json = JsonConvert.SerializeObject(this, Formatting.Indented);
			File.WriteAllText(file_path, json);
		}
		catch(Exception ex) {
			// 
		}
	}
}

// END FILE
