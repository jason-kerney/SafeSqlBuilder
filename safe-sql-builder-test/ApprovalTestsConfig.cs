using ApprovalTests.Reporters;

#if SILENT 

[assembly: UseReporter(typeof(QuietReporter))]


#else

[assembly: UseReporter(typeof(DiffReporter))]

#endif