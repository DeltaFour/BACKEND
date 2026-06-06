using DeltaFour.Application.Dtos.TimeSheet;
using DeltaFour.Application.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DeltaFour.Application.Documents;

public class TimeSheetDocument : IDocument
{
    private readonly TimeSheetDataDto _data;

    private static readonly string PrimaryColor        = "#1a365d";
    private static readonly string SecondaryColor      = "#2b6cb0";
    private static readonly string HeaderBgColor       = "#e2e8f0";
    private static readonly string AlternateRowColor   = "#f7fafc";
    private static readonly string AbsentColor         = "#fc8181";
    private static readonly string NegativeBalanceColor = "#e53e3e";
    private static readonly string PositiveBalanceColor = "#38a169";
    private static readonly string WarningBgColor      = "#fef3c7";
    private static readonly string WarningBorderColor  = "#d97706";

    public TimeSheetDocument(TimeSheetDataDto data) => _data = data;

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(20);
            page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

            // Sem page.Header() — o cabeçalho fica dentro do Content
            // para aparecer somente na primeira página
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    // ── Content (tudo aqui: cabeçalho + tabela + assinaturas + certificado) ─

    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            // Cabeçalho (aparece só na página 1 por estar no Content)
            ComposeHeader(column);

            // Tabela de registros de ponto
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(60);
                    columns.ConstantColumn(75);
                    columns.ConstantColumn(50);
                    columns.ConstantColumn(50);
                    columns.ConstantColumn(55);
                    columns.ConstantColumn(55);
                    columns.ConstantColumn(55);
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("Data").FontColor(Colors.White).Bold();
                    header.Cell().Element(HeaderCellStyle).Text("Dia").FontColor(Colors.White).Bold();
                    header.Cell().Element(HeaderCellStyle).Text("Entrada").FontColor(Colors.White).Bold();
                    header.Cell().Element(HeaderCellStyle).Text("Saída").FontColor(Colors.White).Bold();
                    header.Cell().Element(HeaderCellStyle).Text("Trabalhado").FontColor(Colors.White).Bold();
                    header.Cell().Element(HeaderCellStyle).Text("Esperado").FontColor(Colors.White).Bold();
                    header.Cell().Element(HeaderCellStyle).Text("Saldo").FontColor(Colors.White).Bold();
                    header.Cell().Element(HeaderCellStyle).Text("Observação").FontColor(Colors.White).Bold();

                    static IContainer HeaderCellStyle(IContainer c) =>
                        c.Background(PrimaryColor).Padding(3).AlignCenter().AlignMiddle();
                });

                for (int i = 0; i < _data.Days.Count; i++)
                {
                    var day = _data.Days[i];
                    var bg  = GetRowBackgroundColor(day, i % 2 == 1);

                    table.Cell().Element(c => DataCellStyle(c, bg)).Text(day.Date.ToString("dd/MM"));
                    table.Cell().Element(c => DataCellStyle(c, bg)).Text(GetShortDayName(day.DayOfWeek));
                    table.Cell().Element(c => DataCellStyle(c, bg)).Text(TimeSheetCalculator.FormatTimeOnly(day.FirstEntry));
                    table.Cell().Element(c => DataCellStyle(c, bg)).Text(TimeSheetCalculator.FormatTimeOnly(day.LastExit));
                    table.Cell().Element(c => DataCellStyle(c, bg)).Text(FormatWorkedHours(day));
                    table.Cell().Element(c => DataCellStyle(c, bg)).Text(FormatExpectedHours(day));
                    table.Cell().Element(c => DataCellStyle(c, bg, day.Balance)).Text(FormatBalance(day));
                    table.Cell().Element(c => ObservationCellStyle(c, bg, day)).Text(day.Observation);
                }
            });

            column.Item().PaddingTop(8);
            ComposeSummary(column);

            // Preenche o espaço restante e ancora as assinaturas no fundo da página 1
            column.Item().Extend().AlignBottom().Element(ComposeSignatureLines);

            // Página 2: apenas o certificado "Validado por"
            column.Item().PageBreak();
            ComposeValidation(column);
        });
    }

    // ── Cabeçalho estilo Solides ─────────────────────────────────────────────

    private void ComposeHeader(ColumnDescriptor column)
    {
        column.Item()
            .BorderBottom(2).BorderColor(PrimaryColor)
            .PaddingBottom(5)
            .AlignCenter()
            .Text("Folha de Ponto Individual de Trabalho")
            .FontSize(13).Bold().FontColor(PrimaryColor);

        column.Item().PaddingTop(6);

        column.Item().Border(1).BorderColor(Colors.Grey.Medium).Table(table =>
        {
            table.ColumnsDefinition(cols =>
            {
                cols.RelativeColumn(3);
                cols.RelativeColumn(2);
            });

            // Empresa | CNPJ
            table.Cell()
                .BorderBottom(1).BorderRight(1).BorderColor(Colors.Grey.Medium)
                .Padding(4).Column(c =>
                {
                    c.Item().Text("EMPRESA/RAZÃO SOCIAL").FontSize(6).FontColor(Colors.Grey.Darken1);
                    c.Item().Text(_data.Company.Name).FontSize(8).SemiBold();
                });
            table.Cell()
                .BorderBottom(1).BorderColor(Colors.Grey.Medium)
                .Padding(4).Column(c =>
                {
                    c.Item().Text("CPF/CNPJ").FontSize(6).FontColor(Colors.Grey.Darken1);
                    c.Item().Text(_data.Company.Cnpj).FontSize(8).SemiBold();
                });

            // Endereço (colspan 2)
            table.Cell().ColumnSpan(2)
                .BorderBottom(1).BorderColor(Colors.Grey.Medium)
                .Padding(4).Column(c =>
                {
                    c.Item().Text("ENDEREÇO/LOGRADOURO").FontSize(6).FontColor(Colors.Grey.Darken1);
                    c.Item().Text(_data.Company.FullAddress).FontSize(8).SemiBold();
                });

            // Funcionário | Cargo
            table.Cell()
                .BorderBottom(1).BorderRight(1).BorderColor(Colors.Grey.Medium)
                .Padding(4).Column(c =>
                {
                    c.Item().Text("FUNCIONÁRIO").FontSize(6).FontColor(Colors.Grey.Darken1);
                    c.Item().Text(_data.Employee.Name).FontSize(8).SemiBold();
                });
            table.Cell()
                .BorderBottom(1).BorderColor(Colors.Grey.Medium)
                .Padding(4).Column(c =>
                {
                    c.Item().Text("CARGO").FontSize(6).FontColor(Colors.Grey.Darken1);
                    c.Item().Text(_data.Employee.Role).FontSize(8).SemiBold();
                });

            // Turno | Horário | Competência
            table.Cell()
                .BorderRight(1).BorderColor(Colors.Grey.Medium)
                .Padding(4).Column(c =>
                {
                    c.Item().Text("TURNO").FontSize(6).FontColor(Colors.Grey.Darken1);
                    c.Item().Text(_data.Employee.ShiftName).FontSize(8).SemiBold();
                });
            table.Cell()
                .BorderColor(Colors.Grey.Medium)
                .Padding(4).Row(r =>
                {
                    r.RelativeItem().Column(c =>
                    {
                        c.Item().Text("HORÁRIO").FontSize(6).FontColor(Colors.Grey.Darken1);
                        c.Item().Text($"{_data.Employee.ShiftStartTime:HH:mm} às {_data.Employee.ShiftEndTime:HH:mm}").FontSize(8).SemiBold();
                    });
                    r.ConstantItem(1).Background(Colors.Grey.Medium);
                    r.RelativeItem().PaddingLeft(4).Column(c =>
                    {
                        c.Item().Text("COMPETÊNCIA").FontSize(6).FontColor(Colors.Grey.Darken1);
                        c.Item().Text(_data.Period).FontSize(8).SemiBold();
                    });
                });
        });

        if (!_data.Summary.IsMonthComplete)
        {
            column.Item().PaddingTop(5);
            column.Item()
                .Background(WarningBgColor).Border(1).BorderColor(WarningBorderColor).Padding(5)
                .Row(row =>
                {
                    row.AutoItem().PaddingRight(5).Text("⚠").FontSize(10);
                    row.RelativeItem()
                        .Text("ATENÇÃO: O fechamento deste mês ainda não foi concluído. Os dados apresentados são parciais.")
                        .FontSize(8).Bold().FontColor(WarningBorderColor);
                });
        }

        column.Item().PaddingTop(6);
    }

    // ── Resumo ───────────────────────────────────────────────────────────────

    private void ComposeSummary(ColumnDescriptor column)
    {
        column.Item().Background(HeaderBgColor).Padding(7).Column(col =>
        {
            col.Item().Text("RESUMO DO PERÍODO").FontSize(9).Bold().FontColor(PrimaryColor);
            col.Item().PaddingTop(5);

            col.Item().Row(row =>
            {
                SummaryCard(row, "Total Trabalhado",
                    TimeSheetCalculator.FormatTimeSpan(_data.Summary.TotalWorkedHours), PrimaryColor);
                row.ConstantItem(8);
                SummaryCard(row, "Total Esperado",
                    TimeSheetCalculator.FormatTimeSpan(_data.Summary.TotalExpectedHours), PrimaryColor);
                row.ConstantItem(8);
                SummaryCard(row, "Total de Faltas",
                    _data.Summary.TotalAbsences.ToString(),
                    _data.Summary.TotalAbsences > 0 ? NegativeBalanceColor : PrimaryColor);
                row.ConstantItem(8);
                var balanceColor = _data.Summary.FinalBalance >= TimeSpan.Zero
                    ? PositiveBalanceColor : NegativeBalanceColor;
                SummaryCard(row, "Saldo Final",
                    TimeSheetCalculator.FormatTimeSpan(_data.Summary.FinalBalance), balanceColor);
            });
        });
    }

    private static void SummaryCard(RowDescriptor row, string label, string value, string valueColor)
    {
        row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Column(c =>
        {
            c.Item().Text(label).FontSize(7).FontColor(Colors.Grey.Darken1);
            c.Item().Text(value).FontSize(12).Bold().FontColor(valueColor);
        });
    }

    // ── Linhas de assinatura (página 1, fundo) ───────────────────────────────
    // Intencionalmente sem preenchimento — destinadas à assinatura física impressa.
    // A validação digital fica no certificado "Validado por" (página 2).

    private void ComposeSignatureLines(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Height(35);
                col.Item().BorderTop(1).BorderColor(Colors.Black).PaddingTop(4);
                col.Item().AlignCenter().Text("Assinatura da Empresa/RH").FontSize(8);
                col.Item().AlignCenter().Text(_data.Company.Name).FontSize(7).FontColor(Colors.Grey.Darken1);
            });

            row.ConstantItem(50);

            row.RelativeItem().Column(col =>
            {
                col.Item().Height(35);
                col.Item().BorderTop(1).BorderColor(Colors.Black).PaddingTop(4);
                col.Item().AlignCenter().Text("Assinatura do Funcionário").FontSize(8);
                col.Item().AlignCenter().Text(_data.Employee.Name).FontSize(7).FontColor(Colors.Grey.Darken1);
            });
        });
    }

    // ── Certificado "Validado por" (página 2) ────────────────────────────────

    private void ComposeValidation(ColumnDescriptor column)
    {
        column.Item().AlignCenter()
            .Text("Validado por")
            .FontSize(11).Bold().FontColor(PrimaryColor);

        column.Item().PaddingTop(2);

        column.Item()
            .Text("Documento validado por:")
            .FontSize(9).SemiBold().FontColor(SecondaryColor);

        column.Item().PaddingTop(6);

        column.Item().Row(row =>
        {
            row.RelativeItem().Element(c => ValidationCard(c,
                "FUNCIONÁRIO",
                _data.Signature.SignedByEmployee,
                _data.Signature.EmployeeName,
                _data.Employee.Role,
                _data.Signature.EmployeeSignedAt));

            row.ConstantItem(10);

            row.RelativeItem().Element(c => ValidationCard(c,
                "EMPRESA / RH",
                _data.Signature.SignedByHR,
                _data.Signature.HRSignerName,
                _data.Company.Name,
                _data.Signature.HRSignedAt));
        });
    }

    private static IContainer ValidationCard(
        IContainer container,
        string title,
        bool isSigned,
        string signerName,
        string detail,
        DateTime? signedAt)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten1).Column(card =>
        {
            card.Item()
                .Background(HeaderBgColor).Padding(5)
                .Text(title).FontSize(7).Bold().FontColor(PrimaryColor);

            card.Item().Padding(8).Column(info =>
            {
                info.Item().Text(signerName).FontSize(9).Bold();
                info.Item().PaddingTop(3).Text(detail).FontSize(7).FontColor(Colors.Grey.Darken1);

                if (isSigned && signedAt.HasValue)
                {
                    info.Item().PaddingTop(3).Row(r =>
                    {
                        r.AutoItem().Text("Data/Hora: ").FontSize(7).FontColor(Colors.Grey.Darken1);
                        r.AutoItem().Text(signedAt.Value.ToString("dd/MM/yyyy HH:mm")).FontSize(7);
                    });
                }
                else
                {
                    info.Item().PaddingTop(4)
                        .Text("Aguardando validação")
                        .FontSize(8).Italic().FontColor(Colors.Grey.Darken1);
                }
            });
        });

        return container;
    }

    // ── Footer (rodapé simples em todas as páginas) ──────────────────────────

    private void ComposeFooter(IContainer container)
    {
        container.BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(5).Row(row =>
        {
            row.RelativeItem()
                .Text($"Documento gerado em: {_data.GeneratedAt:dd/MM/yyyy 'às' HH:mm}")
                .FontSize(7).FontColor(Colors.Grey.Darken1);

            row.RelativeItem().AlignRight().Text(text =>
            {
                text.Span("Página ").FontSize(7).FontColor(Colors.Grey.Darken1);
                text.CurrentPageNumber().FontSize(7).FontColor(Colors.Grey.Darken1);
                text.Span(" de ").FontSize(7).FontColor(Colors.Grey.Darken1);
                text.TotalPages().FontSize(7).FontColor(Colors.Grey.Darken1);
            });
        });
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static IContainer DataCellStyle(IContainer container, string bgColor) =>
        container
            .Background(bgColor)
            .BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
            .Padding(2).AlignCenter().AlignMiddle();

    private static IContainer DataCellStyle(IContainer container, string bgColor, TimeSpan _) =>
        container
            .Background(bgColor)
            .BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
            .Padding(2).AlignCenter().AlignMiddle();

    private static IContainer ObservationCellStyle(IContainer container, string bgColor, TimeSheetDayDto day)
    {
        var effectiveBg = day.IsAbsent && !day.IsDayOff && !day.IsFutureDay ? AbsentColor : bgColor;
        return container
            .Background(effectiveBg)
            .BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
            .Padding(2).AlignLeft().AlignMiddle();
    }

    private static string GetRowBackgroundColor(TimeSheetDayDto day, bool isAlternate)
    {
        if (day.IsFutureDay) return Colors.Grey.Lighten3;
        if (day.IsAbsent && !day.IsDayOff) return AbsentColor;
        if (day.IsDayOff) return Colors.Blue.Lighten5;
        return isAlternate ? AlternateRowColor : Colors.White;
    }

    private static string GetShortDayName(string dayOfWeek) => dayOfWeek switch
    {
        "Domingo"       => "Dom",
        "Segunda-feira" => "Seg",
        "Terça-feira"   => "Ter",
        "Quarta-feira"  => "Qua",
        "Quinta-feira"  => "Qui",
        "Sexta-feira"   => "Sex",
        "Sábado"        => "Sáb",
        _ => dayOfWeek[..3]
    };

    private static string FormatWorkedHours(TimeSheetDayDto day) =>
        day.IsFutureDay || day.IsDayOff ? "--:--" : TimeSheetCalculator.FormatTimeSpan(day.WorkedHours);

    private static string FormatExpectedHours(TimeSheetDayDto day) =>
        day.IsFutureDay || day.IsDayOff ? "--:--" : TimeSheetCalculator.FormatTimeSpan(day.ExpectedHours);

    private static string FormatBalance(TimeSheetDayDto day) =>
        day.IsFutureDay || day.IsDayOff ? "--:--" : TimeSheetCalculator.FormatTimeSpan(day.Balance);
}
