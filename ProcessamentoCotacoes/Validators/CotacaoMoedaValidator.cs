using FluentValidation;
using ProcessamentoCotacoes.Models;

namespace ProcessamentoCotacoes.Validators
{
    public class CotacaoMoedaValidator : AbstractValidator<CotacaoMoeda>
    {
        public CotacaoMoedaValidator()
        {
            RuleFor(c => c.Codigo).NotEmpty().WithMessage("Preencha o campo 'Codigo'")
                .Length(3).WithMessage("O campo 'Codigo' deve possuir 3 caracteres");

            RuleFor(c => c.Valor).NotEmpty().WithMessage("Preencha o campo 'Valor'")
                .GreaterThan(0).WithMessage("O campo 'Valor' deve ser maior do 0");
        }                
    }
}