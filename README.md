# Word to PDF Converter
![Word to PDF Converter](PDF.JPG)
SharePoint 2010 solution feature to allow users to convert their Word document (*.docx) into a PDF file in a document library.

There are two version of Word to PDF converter as specified below. Choose which one is appropriate for the client’s requirement.

### 1. rcr.wordpdfconverter.wsp
Converts a Word document to a PDF file by invoking this feature from the custom menu.

### 2. rcr.wf.pdfconverter.wsp
Seqentiual workflow PDF converter that runs in the background to convert Word document to a PDF file either by:
- New word document is added
- Existing word document is updated
- Manually run the workflow

## References
For more details read the ![As Built document](WordPDFConverter-AsBuilt.pdf)
