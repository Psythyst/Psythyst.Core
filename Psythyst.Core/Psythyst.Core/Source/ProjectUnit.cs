using System;
using System.Collections.Generic;
using System.Linq;

using Psythyst;

namespace Psythyst.Core
{
    /// <summary>
    /// ProjectUnit Class.
    /// </summary>
    public class ProjectUnit<TSource, TResult> : IProjectUnit<TSource, TResult>
    {
        List<IGenerator<TSource, TResult>> _Generator = new List<IGenerator<TSource, TResult>>();
        List<IPostProcessor<TResult>> _PostProcessor = new List<IPostProcessor<TResult>>();
 
        public IProjectUnit<TSource, TResult> AddGenerator(IGenerator<TSource, TResult> Generator, bool Condition = true)
        {
            if (Condition && Generator != null) _Generator.Add(Generator); return this;
        }

        public IProjectUnit<TSource, TResult> AddGeneratorCollection(IEnumerable<IGenerator<TSource, TResult>> Collection, bool Condition = true)
        {
            if(Condition && Collection != null) Collection.Each(x => { if (x != null) _Generator.Add(x); }); return this;
        }

        public IProjectUnit<TSource, TResult> AddPostProcessor(IPostProcessor<TResult> PostProcessor, bool Condition = true)
        {
            if(Condition && PostProcessor != null) _PostProcessor.Add(PostProcessor); return this;
        }

        public IProjectUnit<TSource, TResult> AddPostProcessorCollection(IEnumerable<IPostProcessor<TResult>> Collection, bool Condition = true)
        {
            if(Condition && Collection != null) Collection.Each(x => { if (x != null) _PostProcessor.Add(x); }); return this;
        }

        public IEnumerable<TResult> RunGenerator(TSource Model, bool Order = true, Action<IGenerator<TSource, TResult>, Exception> OnError = null)
        {
            return RunGenerator(Model, _Generator, Order, OnError);
        }

        public IEnumerable<TResult> RunPostProcessor(IEnumerable<TResult> ResultCollection, bool Order = true, Action<IPostProcessor<TResult>, Exception> OnError = null)
        {
            return RunPostProcessor(ResultCollection, _PostProcessor, Order, OnError);
        }

        public IEnumerable<TResult> Run(TSource Model, bool OrderGenerator = true, bool OrderPostProcessor = true, Action<IGenerator<TSource, TResult>, Exception> OnGeneratorError = null, Action<IPostProcessor<TResult>, Exception> OnPostProcessorError = null)
        {
            return Run(Model, _Generator, _PostProcessor, OrderGenerator, OrderPostProcessor, OnGeneratorError, OnPostProcessorError);
        }

        public static IProjectUnit<TSource, TResult> Create()
        {
            return new ProjectUnit<TSource, TResult>();
        }

        public static IEnumerable<TResult> RunGenerator(TSource Model, IEnumerable<IGenerator<TSource, TResult>> Collection, bool Order = true, Action<IGenerator<TSource, TResult>, Exception> OnError = null)
        {
            var _ResultList = new List<TResult>();
            var _GeneratorCollection = (Order) ? Collection.OrderBy(x => x.Priority) : Collection;

            foreach (var Generator in _GeneratorCollection)
            {
                try 
                { 
                    var _Result = Generator.Generate(Model); 
                    _ResultList.AddRange(_Result);
                }
                catch (Exception Error) 
                {
                    if(OnError != null) OnError.Invoke(Generator, Error);
                }
            }

            return _ResultList;
        }

        public static IEnumerable<TResult> RunPostProcessor(IEnumerable<TResult> ResultCollection, IEnumerable<IPostProcessor<TResult>> PostProcessorCollection, bool Order = true, Action<IPostProcessor<TResult>, Exception> OnError = null)
        {
            var _PostProcessorCollection = (Order) ? PostProcessorCollection.OrderByDescending(x => x.Priority) : PostProcessorCollection;
            var Current = ResultCollection; 

            foreach (var PostProcessor in _PostProcessorCollection)
            {
                try 
                { 
                    Current = PostProcessor.Process(Current); 
                }
                catch (Exception Error) 
                {
                    if(OnError != null) OnError.Invoke(PostProcessor, Error);
                }
            }

            return Current;
        }

        public static IEnumerable<TResult> Run(TSource Model, IEnumerable<IGenerator<TSource, TResult>> GeneratorCollection, IEnumerable<IPostProcessor<TResult>> PostProcessorCollection, bool OrderGenerator = true, bool OrderPostProcessor = true, Action<IGenerator<TSource, TResult>, Exception> OnGeneratorError = null, Action<IPostProcessor<TResult>, Exception> OnPostProcessorError = null)
        {
            var _GeneratorResultCollection = RunGenerator(Model, GeneratorCollection, OrderGenerator, OnGeneratorError);
            var _PostProcessorResultCollection = RunPostProcessor(_GeneratorResultCollection, PostProcessorCollection, OrderPostProcessor, OnPostProcessorError);

            return _PostProcessorResultCollection;
        }
    }
}