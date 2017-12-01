﻿using System;
using System.Collections.Generic;
using System.Linq;
using InquirerCS.Components;
using InquirerCS.Interfaces;
using InquirerCS.Questions;

namespace InquirerCS.Builders
{
    public class CheckboxBuilder<TResult> where TResult : IComparable
    {
        private IConfirmComponent<List<TResult>> _confirmComponent;

        private Func<IConfirmComponent<List<TResult>>> _confirmComponentFn = () => { return null; };

        private IConvertToStringComponent<TResult> _convertToStringComponent;

        private Func<IConvertToStringComponent<TResult>> _convertToStringComponentFn = () => { return null; };

        private IDefaultValueComponent<List<TResult>> _defaultValueComponent;

        private Func<IDefaultValueComponent<List<TResult>>> _defaultValueComponentFn = () => { return null; };

        private IDisplayQuestionComponent _displayQuestionComponent;

        private IDisplayErrorComponent _errorComponent;

        private IWaitForInputComponent<ConsoleKey> _inputComponent;

        private string _message;

        private IParseComponent<List<Selectable<TResult>>, List<TResult>> _parseComponent;

        private IRenderChoices<TResult> _renderchoices;

        private List<Selectable<TResult>> _selectedChoices;

        private IValidateComponent<List<TResult>> _validateComponent;

        public CheckboxBuilder(string message, IEnumerable<TResult> choices)
        {
            _message = message;
            _selectedChoices = choices.Select(item => new Selectable<TResult>(false, item)).ToList();
        }

        public CheckboxBuilder<TResult> ConvertToString(Func<TResult, string> convertFn)
        {
            _convertToStringComponentFn = () =>
            {
                return new ConvertToStringComponent<TResult>(convertFn);
            };

            return this;
        }

        public CheckboxBuilder<TResult> WithConfirmation()
        {
            _confirmComponentFn = () =>
            {
                return new ConfirmListComponent<List<TResult>, TResult>(_convertToStringComponent);
            };

            return this;
        }

        public CheckboxBuilder<TResult> WithDefaultValue(IEnumerable<TResult> defaultValues)
        {
            _defaultValueComponentFn = () =>
            {
                return new DefaultValueComponent<List<TResult>>(defaultValues.ToList());
            };

            return this;
        }

        public CheckboxBuilder<TResult> WithDefaultValue(List<TResult> defaultValues)
        {
            _defaultValueComponentFn = () =>
            {
                return new DefaultSelectedValueComponent<TResult>(_selectedChoices, defaultValues);
            };

            return this;
        }

        public CheckboxBuilder<TResult> WithDefaultValue(TResult defaultValues)
        {
            _defaultValueComponentFn = () =>
            {
                return new DefaultSelectedValueComponent<TResult>(_selectedChoices, new List<TResult>() { defaultValues });
            };

            return this;
        }

        public List<TResult> Prompt()
        {
            _convertToStringComponent = _convertToStringComponentFn() ?? new ConvertToStringComponent<TResult>();
            _defaultValueComponent = _defaultValueComponentFn() ?? new DefaultValueComponent<List<TResult>>();
            _confirmComponent = _confirmComponentFn() ?? new NoConfirmationComponent<List<TResult>>();

            _displayQuestionComponent = new DisplayListQuestion<List<TResult>, TResult>(_message, _convertToStringComponent, _defaultValueComponent);

            _inputComponent = new ReadConsoleKey();
            _parseComponent = new ParseSelectableListComponent<List<TResult>, TResult>(_selectedChoices);
            _renderchoices = new DisplaySelectableChoices<TResult>(_selectedChoices, _convertToStringComponent);
            _validateComponent = new ValidationComponent<List<TResult>>();
            _errorComponent = new DisplayErrorCompnent();

            return new Checkbox<List<TResult>, TResult>(_selectedChoices, _confirmComponent, _displayQuestionComponent, _inputComponent, _parseComponent, _renderchoices, _validateComponent, _errorComponent).Prompt();
        }
    }
}