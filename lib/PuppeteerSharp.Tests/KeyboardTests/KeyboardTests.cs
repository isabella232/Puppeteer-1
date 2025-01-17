using System.Collections.Generic;
using System.Threading.Tasks;
using CefSharp.Puppeteer;
using CefSharp.Puppeteer.Input;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.KeyboardTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class KeyboardTests : PuppeteerPageBaseTest
    {
        public KeyboardTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should type into a textarea")]
        [PuppeteerFact]
        public async Task ShouldTypeIntoTheTextarea()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");

            var textarea = await DevToolsContext.QuerySelectorAsync("textarea");
            await textarea.TypeAsync("Type in this text!");
            Assert.Equal("Type in this text!", await DevToolsContext.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should move with the arrow keys")]
        [PuppeteerFact]
        public async Task ShouldMoveWithTheArrowKeys()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await DevToolsContext.TypeAsync("textarea", "Hello World!");
            Assert.Equal("Hello World!", await DevToolsContext.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
            for (var i = 0; i < "World!".Length; i++)
            {
                _ = DevToolsContext.Keyboard.PressAsync("ArrowLeft");
            }

            await DevToolsContext.Keyboard.TypeAsync("inserted ");
            Assert.Equal("Hello inserted World!", await DevToolsContext.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
            _ = DevToolsContext.Keyboard.DownAsync("Shift");
            for (var i = 0; i < "inserted ".Length; i++)
            {
                _ = DevToolsContext.Keyboard.PressAsync("ArrowLeft");
            }

            _ = DevToolsContext.Keyboard.UpAsync("Shift");
            await DevToolsContext.Keyboard.PressAsync("Backspace");
            Assert.Equal("Hello World!", await DevToolsContext.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should send a character with ElementHandle.press")]
        [PuppeteerFact]
        public async Task ShouldSendACharacterWithElementHandlePress()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            var textarea = await DevToolsContext.QuerySelectorAsync("textarea");
            await textarea.PressAsync("a");
            Assert.Equal("a", await DevToolsContext.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));

            await DevToolsContext.EvaluateExpressionAsync("window.addEventListener('keydown', e => e.preventDefault(), true)");

            await textarea.PressAsync("b");
            Assert.Equal("a", await DevToolsContext.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "ElementHandle.press should support |text| option")]
        [PuppeteerFact]
        public async Task ElementHandlePressShouldSupportTextOption()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            var textarea = await DevToolsContext.QuerySelectorAsync("textarea");
            await textarea.PressAsync("a", new PressOptions { Text = "ё" });
            Assert.Equal("ё", await DevToolsContext.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should send a character with sendCharacter")]
        [PuppeteerFact]
        public async Task ShouldSendACharacterWithSendCharacter()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await DevToolsContext.FocusAsync("textarea");
            await DevToolsContext.Keyboard.SendCharacterAsync("嗨");
            Assert.Equal("嗨", await DevToolsContext.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
            await DevToolsContext.EvaluateExpressionAsync("window.addEventListener('keydown', e => e.preventDefault(), true)");
            await DevToolsContext.Keyboard.SendCharacterAsync("a");
            Assert.Equal("嗨a", await DevToolsContext.EvaluateExpressionAsync<string>("document.querySelector('textarea').value"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should report shiftKey")]
        [PuppeteerFact]
        public async Task ShouldReportShiftKey()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/keyboard.html");
            var keyboard = DevToolsContext.Keyboard;
            var codeForKey = new Dictionary<string, int> { ["Shift"] = 16, ["Alt"] = 18, ["Control"] = 17 };
            foreach (var modifier in codeForKey)
            {
                await keyboard.DownAsync(modifier.Key);
                Assert.Equal($"Keydown: {modifier.Key} {modifier.Key}Left {modifier.Value} [{modifier.Key}]", await DevToolsContext.EvaluateExpressionAsync<string>("getResult()"));
                await keyboard.DownAsync("!");
                // Shift+! will generate a keypress
                if (modifier.Key == "Shift")
                {
                    Assert.Equal($"Keydown: ! Digit1 49 [{modifier.Key}]\nKeypress: ! Digit1 33 33 [{modifier.Key}]", await DevToolsContext.EvaluateExpressionAsync<string>("getResult()"));
                }
                else
                {
                    Assert.Equal($"Keydown: ! Digit1 49 [{modifier.Key}]", await DevToolsContext.EvaluateExpressionAsync<string>("getResult()"));
                }

                await keyboard.UpAsync("!");
                Assert.Equal($"Keyup: ! Digit1 49 [{modifier.Key}]", await DevToolsContext.EvaluateExpressionAsync<string>("getResult()"));
                await keyboard.UpAsync(modifier.Key);
                Assert.Equal($"Keyup: {modifier.Key} {modifier.Key}Left {modifier.Value} []", await DevToolsContext.EvaluateExpressionAsync<string>("getResult()"));
            }
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should report multiple modifiers")]
        [PuppeteerFact]
        public async Task ShouldReportMultipleModifiers()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/keyboard.html");
            var keyboard = DevToolsContext.Keyboard;
            await keyboard.DownAsync("Control");
            Assert.Equal("Keydown: Control ControlLeft 17 [Control]", await DevToolsContext.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.DownAsync("Alt");
            Assert.Equal("Keydown: Alt AltLeft 18 [Alt Control]", await DevToolsContext.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.DownAsync(";");
            Assert.Equal("Keydown: ; Semicolon 186 [Alt Control]", await DevToolsContext.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.UpAsync(";");
            Assert.Equal("Keyup: ; Semicolon 186 [Alt Control]", await DevToolsContext.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.UpAsync("Control");
            Assert.Equal("Keyup: Control ControlLeft 17 [Alt]", await DevToolsContext.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.UpAsync("Alt");
            Assert.Equal("Keyup: Alt AltLeft 18 []", await DevToolsContext.EvaluateExpressionAsync<string>("getResult()"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should send proper codes while typing")]
        [PuppeteerFact]
        public async Task ShouldSendProperCodesWhileTyping()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/keyboard.html");
            await DevToolsContext.Keyboard.TypeAsync("!");
            Assert.Equal(string.Join("\n", new[] {
                "Keydown: ! Digit1 49 []",
                "Keypress: ! Digit1 33 33 []",
                "Keyup: ! Digit1 49 []" }), await DevToolsContext.EvaluateExpressionAsync<string>("getResult()"));
            await DevToolsContext.Keyboard.TypeAsync("^");
            Assert.Equal(string.Join("\n", new[] {
                "Keydown: ^ Digit6 54 []",
                "Keypress: ^ Digit6 94 94 []",
                "Keyup: ^ Digit6 54 []" }), await DevToolsContext.EvaluateExpressionAsync<string>("getResult()"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should send proper codes while typing with shift")]
        [PuppeteerFact]
        public async Task ShouldSendProperCodesWhileTypingWithShift()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/keyboard.html");
            var keyboard = DevToolsContext.Keyboard;
            await keyboard.DownAsync("Shift");
            await DevToolsContext.Keyboard.TypeAsync("~");
            Assert.Equal(string.Join("\n", new[] {
                "Keydown: Shift ShiftLeft 16 [Shift]",
                "Keydown: ~ Backquote 192 [Shift]", // 192 is ` keyCode
                "Keypress: ~ Backquote 126 126 [Shift]", // 126 is ~ charCode
                "Keyup: ~ Backquote 192 [Shift]" }), await DevToolsContext.EvaluateExpressionAsync<string>("getResult()"));
            await keyboard.UpAsync("Shift");
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should not type canceled events")]
        [PuppeteerFact]
        public async Task ShouldNotTypeCanceledEvents()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await DevToolsContext.FocusAsync("textarea");
            await DevToolsContext.EvaluateExpressionAsync(@"{
              window.addEventListener('keydown', event => {
                event.stopPropagation();
                event.stopImmediatePropagation();
                if (event.key === 'l')
                  event.preventDefault();
                if (event.key === 'o')
                  event.preventDefault();
              }, false);
            }");
            await DevToolsContext.Keyboard.TypeAsync("Hello World!");
            Assert.Equal("He Wrd!", await DevToolsContext.EvaluateExpressionAsync<string>("textarea.value"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should specify repeat property")]
        [PuppeteerFact]
        public async Task ShouldSpecifyRepeatProperty()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await DevToolsContext.FocusAsync("textarea");
            await DevToolsContext.EvaluateExpressionAsync("document.querySelector('textarea').addEventListener('keydown', e => window.lastEvent = e, true)");
            await DevToolsContext.Keyboard.DownAsync("a");
            Assert.False(await DevToolsContext.EvaluateExpressionAsync<bool>("window.lastEvent.repeat"));
            await DevToolsContext.Keyboard.PressAsync("a");
            Assert.True(await DevToolsContext.EvaluateExpressionAsync<bool>("window.lastEvent.repeat"));

            await DevToolsContext.Keyboard.DownAsync("b");
            Assert.False(await DevToolsContext.EvaluateExpressionAsync<bool>("window.lastEvent.repeat"));
            await DevToolsContext.Keyboard.DownAsync("b");
            Assert.True(await DevToolsContext.EvaluateExpressionAsync<bool>("window.lastEvent.repeat"));

            await DevToolsContext.Keyboard.UpAsync("a");
            await DevToolsContext.Keyboard.DownAsync("a");
            Assert.False(await DevToolsContext.EvaluateExpressionAsync<bool>("window.lastEvent.repeat"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should type all kinds of characters")]
        [PuppeteerFact]
        public async Task ShouldTypeAllKindsOfCharacters()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await DevToolsContext.FocusAsync("textarea");
            const string text = "This text goes onto two lines.\nThis character is 嗨.";
            await DevToolsContext.Keyboard.TypeAsync(text);
            Assert.Equal(text, await DevToolsContext.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should specify location")]
        [PuppeteerFact]
        public async Task ShouldSpecifyLocation()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await DevToolsContext.EvaluateExpressionAsync(@"{
              window.addEventListener('keydown', event => window.keyLocation = event.location, true);
            }");
            var textarea = await DevToolsContext.QuerySelectorAsync("textarea");

            await textarea.PressAsync("Digit5");
            Assert.Equal(0, await DevToolsContext.EvaluateExpressionAsync<int>("keyLocation"));

            await textarea.PressAsync("ControlLeft");
            Assert.Equal(1, await DevToolsContext.EvaluateExpressionAsync<int>("keyLocation"));

            await textarea.PressAsync("ControlRight");
            Assert.Equal(2, await DevToolsContext.EvaluateExpressionAsync<int>("keyLocation"));

            await textarea.PressAsync("NumpadSubtract");
            Assert.Equal(3, await DevToolsContext.EvaluateExpressionAsync<int>("keyLocation"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should throw on unknown keys")]
        [PuppeteerFact]
        public async Task ShouldThrowOnUnknownKeys()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => DevToolsContext.Keyboard.PressAsync("NotARealKey"));

            await Assert.ThrowsAsync<KeyNotFoundException>(() => DevToolsContext.Keyboard.PressAsync("ё"));

            await Assert.ThrowsAsync<KeyNotFoundException>(() => DevToolsContext.Keyboard.PressAsync("😊"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should type emoji")]
        [PuppeteerFact]
        public async Task ShouldTypeEmoji()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/textarea.html");
            await DevToolsContext.TypeAsync("textarea", "👹 Tokyo street Japan \uD83C\uDDEF\uD83C\uDDF5");
            Assert.Equal(
                "👹 Tokyo street Japan \uD83C\uDDEF\uD83C\uDDF5",
                await DevToolsContext.QuerySelectorAsync("textarea").EvaluateFunctionAsync<string>("t => t.value"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should type emoji into an iframe")]
        [PuppeteerFact]
        public async Task ShouldTypeEmojiIntoAniframe()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(DevToolsContext, "emoji-test", TestConstants.ServerUrl + "/input/textarea.html");
            var frame = DevToolsContext.FirstChildFrame();
            var textarea = await frame.QuerySelectorAsync("textarea");
            await textarea.TypeAsync("👹 Tokyo street Japan \uD83C\uDDEF\uD83C\uDDF5");
            Assert.Equal(
                "👹 Tokyo street Japan \uD83C\uDDEF\uD83C\uDDF5",
                await frame.QuerySelectorAsync("textarea").EvaluateFunctionAsync<string>("t => t.value"));
        }

        [PuppeteerTest("keyboard.spec.ts", "Keyboard", "should press the metaKey")]
        [PuppeteerFact]
        public async Task ShouldPressTheMetaKey()
        {
            await DevToolsContext.EvaluateFunctionAsync(@"() =>
            {
                window.result = null;
                document.addEventListener('keydown', event => {
                    window.result = [event.key, event.code, event.metaKey];
                });
            }");
            await DevToolsContext.Keyboard.PressAsync("Meta");
            const int key = 0;
            const int code = 1;
            const int metaKey = 2;
            var result = await DevToolsContext.EvaluateExpressionAsync<object[]>("result");
            Assert.Equal("Meta", result[key]);
            Assert.Equal("MetaLeft", result[code]);
            Assert.Equal(true, result[metaKey]);
        }
    }
}
