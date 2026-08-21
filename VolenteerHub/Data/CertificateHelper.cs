using System;
using System.Diagnostics;
using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using VolenteerHub.Models;

namespace VolenteerHub.Data
{
    public static class CertificateHelper
    {
        // =====================================================
        // PDFSHARP FONT CONFIGURATION
        // =====================================================

        static CertificateHelper()
        {
            // Allow PDFsharp to use fonts installed on Windows.
            GlobalFontSettings.UseWindowsFontsUnderWindows =
                true;
        }


        // =====================================================
        // GENERATE CERTIFICATE
        // =====================================================

        public static string GenerateCertificate(
            User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(
                    "user");
            }


            string verificationStatus =
                VerificationHelper
                    .GetVerificationStatus(
                        user.Id);


            if (verificationStatus !=
                "Verified")
            {
                throw new InvalidOperationException(
                    "Only verified volunteers can generate a certificate.");
            }


            double totalHours =
                DatabaseHelper
                    .GetTotalVolunteerHours(
                        user.Id);


            int joinedEvents =
                DatabaseHelper
                    .GetJoinedEventCount(
                        user.Id);


            string verifiedAt =
                VerificationHelper
                    .GetVerifiedAt(
                        user.Id);


            string folder =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.MyDocuments),
                    "VolunteerHub Certificates");


            if (!Directory.Exists(
                    folder))
            {
                Directory.CreateDirectory(
                    folder);
            }


            string safeName =
                MakeSafeFileName(
                    user.FullName);


            string filePath =
                Path.Combine(
                    folder,
                    "VolunteerHub_Certificate_" +
                    safeName +
                    ".pdf");


            CreatePdf(
                filePath,
                user,
                totalHours,
                joinedEvents,
                verifiedAt);


            return filePath;
        }


        // =====================================================
        // CREATE PDF
        // =====================================================

        private static void CreatePdf(
            string filePath,
            User user,
            double totalHours,
            int joinedEvents,
            string verifiedAt)
        {
            PdfDocument document =
                new PdfDocument();


            document.Info.Title =
                "VolunteerHub Certificate - " +
                user.FullName;


            document.Info.Author =
                "VolunteerHub";


            document.Info.Subject =
                "Certificate of Volunteering";


            PdfPage page =
                document.AddPage();


            page.Orientation =
                PdfSharp.PageOrientation.Landscape;


            page.Size =
                PdfSharp.PageSize.A4;


            XGraphics graphics =
                XGraphics.FromPdfPage(
                    page);


            double width =
                page.Width.Point;


            double height =
                page.Height.Point;


            // =================================================
            // COLORS
            // =================================================

            XColor darkGreen =
                XColor.FromArgb(
                    6,
                    78,
                    59);


            XColor green =
                XColor.FromArgb(
                    8,
                    122,
                    97);


            XColor lightGreen =
                XColor.FromArgb(
                    238,
                    248,
                    243);


            XColor textGray =
                XColor.FromArgb(
                    91,
                    108,
                    101);


            XColor gold =
                XColor.FromArgb(
                    214,
                    157,
                    50);


            // =================================================
            // BACKGROUND
            // =================================================

            graphics.DrawRectangle(
                new XSolidBrush(
                    lightGreen),
                0,
                0,
                width,
                height);


            // =================================================
            // MAIN WHITE CERTIFICATE
            // =================================================

            double outerMargin =
                34;


            graphics.DrawRoundedRectangle(
                XBrushes.White,
                outerMargin,
                outerMargin,
                width -
                (outerMargin * 2),
                height -
                (outerMargin * 2),
                16,
                16);


            // =================================================
            // OUTER BORDER
            // =================================================

            XPen outerBorder =
                new XPen(
                    darkGreen,
                    7);


            graphics.DrawRectangle(
                outerBorder,
                outerMargin + 12,
                outerMargin + 12,
                width -
                ((outerMargin + 12) * 2),
                height -
                ((outerMargin + 12) * 2));


            // =================================================
            // INNER BORDER
            // =================================================

            XPen innerBorder =
                new XPen(
                    green,
                    1.5);


            graphics.DrawRectangle(
                innerBorder,
                outerMargin + 24,
                outerMargin + 24,
                width -
                ((outerMargin + 24) * 2),
                height -
                ((outerMargin + 24) * 2));


            // =================================================
            // FONTS
            // =================================================

            XFont brandFont =
                new XFont(
                    "Segoe UI",
                    22,
                    XFontStyleEx.Bold);


            XFont smallHeadingFont =
                new XFont(
                    "Segoe UI",
                    10,
                    XFontStyleEx.Bold);


            XFont titleFont =
                new XFont(
                    "Segoe UI",
                    31,
                    XFontStyleEx.Bold);


            XFont normalFont =
                new XFont(
                    "Segoe UI",
                    12,
                    XFontStyleEx.Regular);


            XFont nameFont =
                new XFont(
                    "Segoe UI",
                    28,
                    XFontStyleEx.Bold);


            XFont statNumberFont =
                new XFont(
                    "Segoe UI",
                    21,
                    XFontStyleEx.Bold);


            XFont statLabelFont =
                new XFont(
                    "Segoe UI",
                    8,
                    XFontStyleEx.Bold);


            XFont verifiedFont =
                new XFont(
                    "Segoe UI",
                    13,
                    XFontStyleEx.Bold);


            XFont footerFont =
                new XFont(
                    "Segoe UI",
                    9,
                    XFontStyleEx.Regular);


            // =================================================
            // BRAND
            // =================================================

            graphics.DrawString(
                "VolunteerHub",
                brandFont,
                new XSolidBrush(
                    green),
                new XRect(
                    0,
                    70,
                    width,
                    35),
                XStringFormats.TopCenter);


            // =================================================
            // CERTIFICATE LABEL
            // =================================================

            graphics.DrawString(
                "CERTIFICATE OF VOLUNTEERING",
                smallHeadingFont,
                new XSolidBrush(
                    textGray),
                new XRect(
                    0,
                    116,
                    width,
                    22),
                XStringFormats.TopCenter);


            // =================================================
            // TITLE
            // =================================================

            graphics.DrawString(
                "Certificate of Impact",
                titleFont,
                new XSolidBrush(
                    darkGreen),
                new XRect(
                    0,
                    142,
                    width,
                    50),
                XStringFormats.TopCenter);


            // =================================================
            // PRESENTED TO
            // =================================================

            graphics.DrawString(
                "This certificate is proudly presented to",
                normalFont,
                new XSolidBrush(
                    textGray),
                new XRect(
                    0,
                    205,
                    width,
                    25),
                XStringFormats.TopCenter);


            // =================================================
            // VOLUNTEER NAME
            // =================================================

            graphics.DrawString(
                user.FullName,
                nameFont,
                new XSolidBrush(
                    green),
                new XRect(
                    80,
                    236,
                    width - 160,
                    45),
                XStringFormats.TopCenter);


            // =================================================
            // DECORATIVE LINE
            // =================================================

            double lineWidth =
                330;


            double lineStart =
                (width -
                 lineWidth) /
                2;


            graphics.DrawLine(
                new XPen(
                    gold,
                    1.5),
                lineStart,
                285,
                lineStart +
                lineWidth,
                285);


            // =================================================
            // DESCRIPTION
            // =================================================

            graphics.DrawString(
                "In recognition of dedication, contribution and volunteer service through VolunteerHub.",
                normalFont,
                new XSolidBrush(
                    textGray),
                new XRect(
                    80,
                    305,
                    width - 160,
                    35),
                XStringFormats.TopCenter);


            // =================================================
            // STAT CARDS
            // =================================================

            double statWidth =
                170;


            double statHeight =
                82;


            double statGap =
                24;


            double totalStatsWidth =
                (statWidth * 2) +
                statGap;


            double statsStartX =
                (width -
                 totalStatsWidth) /
                2;


            double statsY =
                360;


            XBrush statsBackground =
                new XSolidBrush(
                    XColor.FromArgb(
                        244,
                        249,
                        247));


            graphics.DrawRoundedRectangle(
                statsBackground,
                statsStartX,
                statsY,
                statWidth,
                statHeight,
                10,
                10);


            graphics.DrawRoundedRectangle(
                statsBackground,
                statsStartX +
                statWidth +
                statGap,
                statsY,
                statWidth,
                statHeight,
                10,
                10);


            // =================================================
            // HOURS
            // =================================================

            graphics.DrawString(
                totalHours.ToString(
                    "0.##") +
                " h",
                statNumberFont,
                new XSolidBrush(
                    darkGreen),
                new XRect(
                    statsStartX,
                    statsY + 14,
                    statWidth,
                    30),
                XStringFormats.TopCenter);


            graphics.DrawString(
                "VOLUNTEER HOURS",
                statLabelFont,
                new XSolidBrush(
                    textGray),
                new XRect(
                    statsStartX,
                    statsY + 50,
                    statWidth,
                    20),
                XStringFormats.TopCenter);


            // =================================================
            // EVENTS
            // =================================================

            double secondStatX =
                statsStartX +
                statWidth +
                statGap;


            graphics.DrawString(
                joinedEvents.ToString(),
                statNumberFont,
                new XSolidBrush(
                    darkGreen),
                new XRect(
                    secondStatX,
                    statsY + 14,
                    statWidth,
                    30),
                XStringFormats.TopCenter);


            graphics.DrawString(
                "JOINED EVENTS",
                statLabelFont,
                new XSolidBrush(
                    textGray),
                new XRect(
                    secondStatX,
                    statsY + 50,
                    statWidth,
                    20),
                XStringFormats.TopCenter);


            // =================================================
            // VERIFIED STATUS
            // =================================================

            graphics.DrawString(
                "VERIFIED VOLUNTEER",
                verifiedFont,
                new XSolidBrush(
                    green),
                new XRect(
                    0,
                    467,
                    width,
                    25),
                XStringFormats.TopCenter);


            if (!string.IsNullOrWhiteSpace(
                    verifiedAt))
            {
                graphics.DrawString(
                    "Identity verified: " +
                    verifiedAt,
                    footerFont,
                    new XSolidBrush(
                        textGray),
                    new XRect(
                        0,
                        492,
                        width,
                        18),
                    XStringFormats.TopCenter);
            }


            // =================================================
            // ISSUE DATE
            // =================================================

            string issueDate =
                DateTime.Today.ToString(
                    "dd MMMM yyyy");


            graphics.DrawString(
                "Issued by VolunteerHub on " +
                issueDate,
                footerFont,
                new XSolidBrush(
                    textGray),
                new XRect(
                    0,
                    height - 93,
                    width,
                    18),
                XStringFormats.TopCenter);


            graphics.DrawString(
                "Make an impact.",
                footerFont,
                new XSolidBrush(
                    green),
                new XRect(
                    0,
                    height - 72,
                    width,
                    18),
                XStringFormats.TopCenter);


            // =================================================
            // SAVE PDF
            // =================================================

            document.Save(
                filePath);


            document.Close();
        }


        // =====================================================
        // SAFE FILE NAME
        // =====================================================

        private static string MakeSafeFileName(
            string value)
        {
            if (string.IsNullOrWhiteSpace(
                    value))
            {
                return "Volunteer";
            }


            string result =
                value
                    .Trim()
                    .Replace(
                        " ",
                        "_");


            char[] invalidCharacters =
                Path.GetInvalidFileNameChars();


            foreach (
                char invalidCharacter
                in invalidCharacters)
            {
                result =
                    result.Replace(
                        invalidCharacter,
                        '_');
            }


            return result;
        }


        // =====================================================
        // OPEN PDF
        // =====================================================

        public static void OpenCertificate(
            string filePath)
        {
            if (string.IsNullOrWhiteSpace(
                    filePath) ||
                !File.Exists(
                    filePath))
            {
                return;
            }


            ProcessStartInfo processInfo =
                new ProcessStartInfo();


            processInfo.FileName =
                filePath;


            processInfo.UseShellExecute =
                true;


            Process.Start(
                processInfo);
        }
    }
}