﻿using System;
using InquirerCS.Components;
using InquirerCS.Interfaces;
using InquirerCS.Questions;

namespace InquirerCS.Builders
{
    public class ConfirmBuilder : Builder<InputKey<bool>, ConsoleKey, bool>
    {
        private string _message;

        private IOnKey _onKey;

        public ConfirmBuilder(string message)
        {
            _message = message + " [y/n]";
            _validationInputComponent = new ValidationComponent<ConsoleKey>();
            _validationResultComponent = new ValidationComponent<bool>();
        }

        public override InputKey<bool> Build()
        {
            _convertToStringComponent = new ConvertToStringComponent<bool>(value =>
            {
                return value ? "yes" : "no";
            });

            _defaultValueComponent = _defaultValueComponentFn() ?? new DefaultValueComponent<bool>();
            _confirmComponent = _confirmComponentFn() ?? new NoConfirmationComponent<bool>();

            _displayQuestionComponent = new DisplayQuestion<bool>(_message, _convertToStringComponent, _defaultValueComponent);
            _inputComponent = new ReadConsoleKey();
            _parseComponent = new ParseComponent<ConsoleKey, bool>(value =>
            {
                return value == ConsoleKey.Y;
            });

            _errorDisplay = new DisplayErrorCompnent();

            return new InputKey<bool>(_confirmComponent, _displayQuestionComponent, _inputComponent, _parseComponent, _validationResultComponent, _validationInputComponent, _errorDisplay, _defaultValueComponent, _onKey);
        }

        public override bool Prompt()
        {
            return Build().Prompt();
        }

        public ConfirmBuilder WithConfirmation()
        {
            _confirmComponentFn = () =>
            {
                return new ConfirmComponent<bool>(_convertToStringComponentFn());
            };

            return this;
        }

        public ConfirmBuilder WithDefaultValue(bool defaultValues)
        {
            _defaultValueComponentFn = () =>
            {
                return new DefaultValueComponent<bool>(defaultValues);
            };

            return this;
        }

        public ConfirmBuilder WithValidation(Func<bool, bool> fn, Func<bool, string> errorMessageFn)
        {
            _validationResultComponent.Add(fn, errorMessageFn);
            return this;
        }

        public ConfirmBuilder WithValidation(Func<bool, bool> fn, string errorMessage)
        {
            _validationResultComponent.Add(fn, errorMessage);
            return this;
        }
    }
}
