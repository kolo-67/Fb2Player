using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fb2PlayerViewModel.Infrostructure
{
    public class SpeechSynthesizerAsTask
    {
        SpeechSynthesizer synthesizer;
        Prompt results = null;
        TaskCompletionSource<Prompt> tcs;
        public SpeechSynthesizerAsTask()
        {
            synthesizer = new SpeechSynthesizer();
            EventHandler<SpeakCompletedEventArgs> del = (obj, args) =>
            {
                if (args.Cancelled)
                    tcs.SetCanceled();
                else if (args.Error != null)
                    tcs.SetException(args.Error);
                else
                    tcs.TrySetResult(results);
            };
            synthesizer.SpeakCompleted += del;
        }

        public Task<Prompt> SpeakAsync(object speech, CancellationToken token)
        {
            tcs = new TaskCompletionSource<Prompt>(TaskCreationOptions.AttachedToParent);

            token.Register(() =>
            {
                synthesizer.SpeakAsyncCancel(results);
            });


            EventHandler<SpeakCompletedEventArgs> del = (obj, args) =>
            {
                if (args.Cancelled)
                    tcs.TrySetResult(results);      //tcs.SetCanceled();
                else if (args.Error != null)
                    tcs.SetException(args.Error);
                else
                    tcs.TrySetResult(results);
            };
            synthesizer.SpeakCompleted += del;

            if (speech is string)
                results = synthesizer.SpeakAsync(speech as string);
            else
            {
                results = (Prompt)speech;
                synthesizer.SpeakAsync(results);
            }

            return tcs.Task;
        }

    }
}
