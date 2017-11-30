﻿using System;
using System.Collections.Generic;
using InquirerCS.Beta2.Interfaces;

namespace InquirerCS.Beta2.Questions
{
    public class RawList<TResult> : IQuestion<TResult>
    {
        private List<TResult> _choices;

        private IConfirmComponent<TResult> _confirmComponent;

        private IDisplayQuestionComponent _displayQuestion;

        private IDisplayErrorComponent _errorComponent;

        private IWaitForInputComponent<string> _inputComponent;

        private IParseComponent<string, TResult> _parseComponent;

        private IRenderchoices<TResult> _renderChoices;

        private IValidateComponent<string> _validationInputComponent;

        private IValidateComponent<TResult> _validationResultComponent;

        public RawList(
            List<TResult> choices,
            IConfirmComponent<TResult> confirmComponent,
            IDisplayQuestionComponent displayQuestion,
            IWaitForInputComponent<string> inputComponent,
            IParseComponent<string, TResult> parseComponent,
            IRenderchoices<TResult> renderChoices,
            IValidateComponent<TResult> validationResultComponent,
            IValidateComponent<string> validationInputComponent,
            IDisplayErrorComponent errorComponent)
        {
            _choices = choices;
            _confirmComponent = confirmComponent;
            _displayQuestion = displayQuestion;
            _inputComponent = inputComponent;
            _parseComponent = parseComponent;
            _renderChoices = renderChoices;
            _validationInputComponent = validationInputComponent;
            _validationResultComponent = validationResultComponent;
            _errorComponent = errorComponent;

            Console.CursorVisible = false;
        }

        public TResult Prompt()
        {
            _displayQuestion.Render();
            _renderChoices.Render();

            var value = _inputComponent.WaitForInput();

            var validationResult = _validationInputComponent.Run(value);
            if (validationResult.HasError)
            {
                _errorComponent.Render(validationResult.ErrorMessage);
                return Prompt();
            }

            TResult result = _parseComponent.Parse(value);
            validationResult = _validationResultComponent.Run(result);
            if (validationResult.HasError)
            {
                _errorComponent.Render(validationResult.ErrorMessage);
                return Prompt();
            }

            if (_confirmComponent.Confirm(result))
            {
                return Prompt();
            }

            return result;
        }
    }
}
