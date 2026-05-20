using DeltaFour.Application.Dtos.TimeSheet;
using DeltaFour.Application.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DeltaFour.Application.Documents;

/// <summary>
/// Documento QuestPDF para geração da folha de ponto em PDF
/// </summary>
public class TimeSheetDocument : IDocument
{
    private readonly TimeSheetDataDto _data;

    // Cores do tema
    private static readonly string PrimaryColor = "#1a365d";      // Azul escuro
    private static readonly string SecondaryColor = "#2b6cb0";    // Azul médio
    private static readonly string HeaderBgColor = "#e2e8f0";     // Cinza claro
    private static readonly string AlternateRowColor = "#f7fafc"; // Cinza muito claro
    private static readonly string AbsentColor = "#fc8181";       // Vermelho claro
    private static readonly string NegativeBalanceColor = "#e53e3e"; // Vermelho
    private static readonly string PositiveBalanceColor = "#38a169"; // Verde
    private static readonly string WarningBgColor = "#fef3c7";    // Amarelo claro
    private static readonly string WarningBorderColor = "#d97706"; // Laranja

    public TimeSheetDocument(TimeSheetDataDto data)
    {
        _data = data;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(25);
            page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            // Título principal
            column.Item().BorderBottom(2).BorderColor(PrimaryColor).PaddingBottom(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("FOLHA DE PONTO")
                        .FontSize(18)
                        .Bold()
                        .FontColor(PrimaryColor);

                    col.Item().Text($"Competência: {_data.Period}")
                        .FontSize(11)
                        .SemiBold()
                        .FontColor(SecondaryColor);
                });
            });

            column.Item().PaddingTop(10);

            // Dados da empresa
            column.Item().Background(HeaderBgColor).Padding(10).Column(col =>
            {
                col.Item().Text("DADOS DA EMPRESA").FontSize(10).Bold().FontColor(PrimaryColor);
                col.Item().PaddingTop(5);

                col.Item().Row(row =>
                {
                    row.RelativeItem(2).Column(c =>
                    {
                        c.Item().Text(text =>
                        {
                            text.Span("Razão Social: ").SemiBold();
                            text.Span(_data.Company.Name);
                        });
                    });

                    row.RelativeItem(1).Column(c =>
                    {
                        c.Item().Text(text =>
                        {
                            text.Span("CNPJ: ").SemiBold();
                            text.Span(_data.Company.Cnpj);
                        });
                    });
                });

                col.Item().Text(text =>
                {
                    text.Span("Endereço: ").SemiBold();
                    text.Span(_data.Company.FullAddress);
                });
            });

            column.Item().PaddingTop(8);

            // Dados do funcionário
            column.Item().Background(HeaderBgColor).Padding(10).Column(col =>
            {
                col.Item().Text("DADOS DO FUNCIONÁRIO").FontSize(10).Bold().FontColor(PrimaryColor);
                col.Item().PaddingTop(5);

                col.Item().Row(row =>
                {
                    row.RelativeItem(2).Text(text =>
                    {
                        text.Span("Nome: ").SemiBold();
                        text.Span(_data.Employee.Name);
                    });

                    row.RelativeItem(1).Text(text =>
                    {
                        text.Span("Cargo: ").SemiBold();
                        text.Span(_data.Employee.Role);
                    });
                });

                col.Item().Row(row =>
                {
                    row.RelativeItem(1).Text(text =>
                    {
                        text.Span("Turno: ").SemiBold();
                        text.Span(_data.Employee.ShiftName);
                    });

                    row.RelativeItem(1).Text(text =>
                    {
                        text.Span("Horário: ").SemiBold();
                        text.Span($"{_data.Employee.ShiftStartTime:HH:mm} às {_data.Employee.ShiftEndTime:HH:mm}");
                    });
                });
            });

            // Aviso de mês incompleto
            if (!_data.Summary.IsMonthComplete)
            {
                column.Item().PaddingTop(10);
                column.Item()
                    .Background(WarningBgColor)
                    .Border(1)
                    .BorderColor(WarningBorderColor)
                    .Padding(8)
                    .Row(row =>
                    {
                        row.AutoItem().PaddingRight(5).Text("⚠").FontSize(12);
                        row.RelativeItem().Text("ATENÇÃO: O fechamento deste mês ainda não foi concluído. Os dados apresentados são parciais.")
                            .FontSize(9)
                            .Bold()
                            .FontColor(WarningBorderColor);
                    });
            }

            column.Item().PaddingTop(10);
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            // Tabela de registros de ponto
            column.Item().Table(table =>
            {
                // Definição das colunas
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(60);  // Data
                    columns.ConstantColumn(75);  // Dia da Semana
                    columns.ConstantColumn(50);  // Entrada
                    columns.ConstantColumn(50);  // Saída
                    columns.ConstantColumn(55);  // Trabalhado
                    columns.ConstantColumn(55);  // Esperado
                    columns.ConstantColumn(55);  // Saldo
                    columns.RelativeColumn();    // Observação
                });

                // Cabeçalho da tabela
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Background(PrimaryColor).Text("Data").FontColor(Colors.White).Bold();
                    header.Cell().Element(CellStyle).Background(PrimaryColor).Text("Dia").FontColor(Colors.White).Bold();
                    header.Cell().Element(CellStyle).Background(PrimaryColor).Text("Entrada").FontColor(Colors.White).Bold();
                    header.Cell().Element(CellStyle).Background(PrimaryColor).Text("Saída").FontColor(Colors.White).Bold();
                    header.Cell().Element(CellStyle).Background(PrimaryColor).Text("Trabalhado").FontColor(Colors.White).Bold();
                    header.Cell().Element(CellStyle).Background(PrimaryColor).Text("Esperado").FontColor(Colors.White).Bold();
                    header.Cell().Element(CellStyle).Background(PrimaryColor).Text("Saldo").FontColor(Colors.White).Bold();
                    header.Cell().Element(CellStyle).Background(PrimaryColor).Text("Observação").FontColor(Colors.White).Bold();

                    static IContainer CellStyle(IContainer container) =>
                        container.Padding(5).AlignCenter().AlignMiddle();
                });

                // Linhas de dados
                for (int i = 0; i < _data.Days.Count; i++)
                {
                    var day = _data.Days[i];
                    var isAlternate = i % 2 == 1;
                    var bgColor = GetRowBackgroundColor(day, isAlternate);

                    table.Cell().Element(c => DataCellStyle(c, bgColor)).Text(day.Date.ToString("dd/MM"));
                    table.Cell().Element(c => DataCellStyle(c, bgColor)).Text(GetShortDayName(day.DayOfWeek));
                    table.Cell().Element(c => DataCellStyle(c, bgColor)).Text(TimeSheetCalculator.FormatTimeOnly(day.FirstEntry));
                    table.Cell().Element(c => DataCellStyle(c, bgColor)).Text(TimeSheetCalculator.FormatTimeOnly(day.LastExit));
                    table.Cell().Element(c => DataCellStyle(c, bgColor)).Text(FormatWorkedHours(day));
                    table.Cell().Element(c => DataCellStyle(c, bgColor)).Text(FormatExpectedHours(day));
                    table.Cell().Element(c => DataCellStyle(c, bgColor, day.Balance)).Text(FormatBalance(day));
                    table.Cell().Element(c => ObservationCellStyle(c, bgColor, day)).Text(day.Observation);
                }
            });

            column.Item().PaddingTop(15);

            // Totalizadores
            ComposeSummary(column);
        });
    }

    private void ComposeSummary(ColumnDescriptor column)
    {
        column.Item().Background(HeaderBgColor).Padding(10).Column(col =>
        {
            col.Item().Text("RESUMO DO PERÍODO").FontSize(11).Bold().FontColor(PrimaryColor);
            col.Item().PaddingTop(8);

            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Column(inner =>
                    {
                        inner.Item().Text("Total Trabalhado").FontSize(8).FontColor(Colors.Grey.Darken1);
                        inner.Item().Text(TimeSheetCalculator.FormatTimeSpan(_data.Summary.TotalWorkedHours))
                            .FontSize(14).Bold().FontColor(PrimaryColor);
                    });
                });

                row.ConstantItem(10);

                row.RelativeItem().Column(c =>
                {
                    c.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Column(inner =>
                    {
                        inner.Item().Text("Total Esperado").FontSize(8).FontColor(Colors.Grey.Darken1);
                        inner.Item().Text(TimeSheetCalculator.FormatTimeSpan(_data.Summary.TotalExpectedHours))
                            .FontSize(14).Bold().FontColor(PrimaryColor);
                    });
                });

                row.ConstantItem(10);

                row.RelativeItem().Column(c =>
                {
                    c.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Column(inner =>
                    {
                        inner.Item().Text("Total de Faltas").FontSize(8).FontColor(Colors.Grey.Darken1);
                        inner.Item().Text(_data.Summary.TotalAbsences.ToString())
                            .FontSize(14).Bold().FontColor(_data.Summary.TotalAbsences > 0 ? NegativeBalanceColor : PrimaryColor);
                    });
                });

                row.ConstantItem(10);

                row.RelativeItem().Column(c =>
                {
                    var balanceColor = _data.Summary.FinalBalance >= TimeSpan.Zero ? PositiveBalanceColor : NegativeBalanceColor;
                    c.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Column(inner =>
                    {
                        inner.Item().Text("Saldo Final").FontSize(8).FontColor(Colors.Grey.Darken1);
                        inner.Item().Text(TimeSheetCalculator.FormatTimeSpan(_data.Summary.FinalBalance))
                            .FontSize(14).Bold().FontColor(balanceColor);
                    });
                });
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().PaddingTop(20);

            // Área de assinaturas
            column.Item().Row(row =>
            {
                // Assinatura da empresa
                row.RelativeItem().Column(col =>
                {
                    col.Item().PaddingTop(40).BorderTop(1).BorderColor(Colors.Black).PaddingTop(5);
                    col.Item().AlignCenter().Text("Assinatura da Empresa").FontSize(9);
                    col.Item().AlignCenter().Text(_data.Company.Name).FontSize(8).FontColor(Colors.Grey.Darken1);
                });

                row.ConstantItem(50);

                // Assinatura do funcionário
                row.RelativeItem().Column(col =>
                {
                    col.Item().PaddingTop(40).BorderTop(1).BorderColor(Colors.Black).PaddingTop(5);
                    col.Item().AlignCenter().Text("Assinatura do Funcionário").FontSize(9);
                    col.Item().AlignCenter().Text(_data.Employee.Name).FontSize(8).FontColor(Colors.Grey.Darken1);
                });
            });

            column.Item().PaddingTop(15);

            // Data de emissão e rodapé
            column.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(5).Row(row =>
            {
                row.RelativeItem().Text($"Documento gerado em: {_data.GeneratedAt:dd/MM/yyyy 'às' HH:mm}")
                    .FontSize(8)
                    .FontColor(Colors.Grey.Darken1);

                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Página ").FontSize(8).FontColor(Colors.Grey.Darken1);
                    text.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Darken1);
                    text.Span(" de ").FontSize(8).FontColor(Colors.Grey.Darken1);
                    text.TotalPages().FontSize(8).FontColor(Colors.Grey.Darken1);
                });
            });
        });
    }

    #region Helper Methods

    private static IContainer DataCellStyle(IContainer container, string bgColor)
    {
        return container
            .Background(bgColor)
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(4)
            .AlignCenter()
            .AlignMiddle();
    }

    private static IContainer DataCellStyle(IContainer container, string bgColor, TimeSpan balance)
    {
        var textColor = balance < TimeSpan.Zero ? NegativeBalanceColor : 
                        balance > TimeSpan.Zero ? PositiveBalanceColor : Colors.Black;

        return container
            .Background(bgColor)
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(4)
            .AlignCenter()
            .AlignMiddle();
    }

    private static IContainer ObservationCellStyle(IContainer container, string bgColor, TimeSheetDayDto day)
    {
        var effectiveBg = day.IsAbsent && !day.IsDayOff && !day.IsFutureDay ? AbsentColor : bgColor;

        return container
            .Background(effectiveBg)
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(4)
            .AlignLeft()
            .AlignMiddle();
    }

    private static string GetRowBackgroundColor(TimeSheetDayDto day, bool isAlternate)
    {
        if (day.IsFutureDay)
        {
            return Colors.Grey.Lighten3;
        }

        if (day.IsAbsent && !day.IsDayOff)
        {
            return AbsentColor;
        }

        if (day.IsDayOff)
        {
            return Colors.Blue.Lighten5;
        }

        return isAlternate ? AlternateRowColor : Colors.White;
    }

    private static string GetShortDayName(string dayOfWeek)
    {
        return dayOfWeek switch
        {
            "Domingo" => "Dom",
            "Segunda-feira" => "Seg",
            "Terça-feira" => "Ter",
            "Quarta-feira" => "Qua",
            "Quinta-feira" => "Qui",
            "Sexta-feira" => "Sex",
            "Sábado" => "Sáb",
            _ => dayOfWeek[..3]
        };
    }

    private static string FormatWorkedHours(TimeSheetDayDto day)
    {
        if (day.IsFutureDay || day.IsDayOff)
        {
            return "--:--";
        }

        return TimeSheetCalculator.FormatTimeSpan(day.WorkedHours);
    }

    private static string FormatExpectedHours(TimeSheetDayDto day)
    {
        if (day.IsFutureDay || day.IsDayOff)
        {
            return "--:--";
        }

        return TimeSheetCalculator.FormatTimeSpan(day.ExpectedHours);
    }

    private static string FormatBalance(TimeSheetDayDto day)
    {
        if (day.IsFutureDay || day.IsDayOff)
        {
            return "--:--";
        }

        return TimeSheetCalculator.FormatTimeSpan(day.Balance);
    }

    #endregion
}
