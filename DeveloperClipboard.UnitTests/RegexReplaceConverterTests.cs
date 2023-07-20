using DeveloperClipboardCore;
using DeveloperClipboardCore.Conventions;

namespace DeveloperClipboard.UnitTests;

[UseReporter(typeof(DiffReporter))]
public class RegexReplaceConverterTests
{
    private readonly RegexReplaceConverter _converter = new();

    #region TestData

    // TODO Убрать специфику проекта
    private const string TypescriptCode = @"export type Range = number[];

// ------------------------------
// Внешний формат данных: получаемый с сервера

/** Тип периода */
export type TermType = 'QUARTILE' | 'TRIMESTER' | 'SEMESTER';

/** Общая информация об ОП */
export interface PlanVersion {
  /** Тип периода */
  termType: TermType;

  /** Навание периода */
  termLabel: string;

  /** Общее кол-во периодов */
  termsTotalNumber: number;

  /** КС */
  courses: ServerCourse[];
}

/** Тип КС */
export type CourseType = typeof CURRICULUM_BLOCK | typeof ACADEMIC_COURSE | typeof PROGRAM;

/** КС (с сервера) */
export interface ServerCourse {
  /** ИД КС */
  id: string;

  type: 'course';

  /** Тип КС */
  courseType: CourseType;

  /**??? Период (с, по) */
  range: Range;

  /** Название */
  name: string;

  /** Кол-во единиц, которые необходимо набрать */
  points: number;

  /** КС обязательна? */
  required: boolean;

  /** Факультатив? */
  faculty: boolean;

  /** Вложенные КС / элементы КС */
  childs: (ServerCourseElement | ServerCourse)[];

  /**??? Привязка к строке на UI */
  row: number | null;
}

/** Элемент КС (с сервера) */
export interface ServerCourseElement {
  /** ИД КС */
  id: string;

  type: 'course-unit';

  /** ИД МУП */
  courseUnitId: string;

  /** ??? Период (с, по) */
  terms: Range;

  /** Название МУП */
  name: string;

  /** Кол-во единиц */
  points: number;

  /** Элемент обязателен? */
  required: boolean;

  /** ??? Привязка к строке на UI */
  row: number | null;
}
";

    private const string CSharpCode = @"/// <summary> КС (с сервера) </summary>
public class ServerCourse
{
    /// <summary> ИД КС </summary>
    public required string Id { get; set; } = default!;

    /// <summary> Тип КС </summary>
    public string CourseType { get; set; } = default!;

    /// <summary> Период (с, по) </summary>
    public int[] Range { get; set; } = default!;

    /// <summary> Название </summary>
    public string Name { get; set; } = default!;

    /// <summary> Кол-во единиц, которые необходимо набрать </summary>
    public int Points { get; set; } = default!;

    /// <summary> Кол-во единиц, которые необходимо набрать </summary>
    public double SomeDouble { get; set; } = default!;

    /// <summary> КС обязательна? </summary>
    public bool Required { get; set; } = default!;

    /// <summary> Факультатив? </summary>
    public bool Faculty { get; set; } = default!;

    /// <summary> Вложенные КС </summary>
    public ServerCourse[] ChildCourses { get; set; } = default!;

    /// <summary> Вложенные элементы КС </summary>
    public ServerCourseElement[] ChildCourseElements { get; set; } = default!;

    /// <summary> Привязка к строке на UI </summary>
    public int? Row { get; set; } = default!;

    /// <summary> КС можно скрыть из ОП студента? </summary>
    public bool CanHide { get; set; } = default!;

    /// <summary> КС скрыта из ОП студента? </summary>
    public bool Hidden { get; set; } = default!;

    /// <summary> КС может быть окном выбора? </summary>
    public bool CanBeElection { get; set; } = default!;

    /// <summary> Данная КС - это окно выбора? (электив) </summary>
    public bool IsElection { get; set; }

    /// <summary>
    /// Связи между МУПами
    /// </summary>
    public Edge[] Links { get; set; } = default!;
}";

    private const string CSharpCode2 = @"/// <summary>Дата создания заявки</summary>
    [DataType(DataType.Date)]
    public DateTime CreatedDate { get; set; }

    /// <summary>Код</summary>
    [MaxLength(1000)]
    public string Code { get; set; }

    /// <summary>Название</summary>
    [MaxLength(1000)]
    [Required]
    public string Name { get; set; }";

    private const string CSharpParams = "string curriculumFlowId, string specialityCode, string profileId";

    private const string DdlCode = @"CREATE TABLE curriculum (
	id varchar(100) NOT NULL, -- Идентификатор ОП
	code varchar(100) NULL, -- Код ОП
	""name"" varchar(1000) NULL, -- Наименование ОП
	description varchar(4000) NULL, -- Описание ОП
	completion_criterion int4 NULL, -- Критерий завершения
	duration numeric(4, 2) NULL, -- Продолжительность обучения, лет
	manager_id varchar(100) NULL, -- Руководитель
	date_start timestamp NULL, -- Дата формирования ОП
	date_end timestamp NULL, -- Дата закрытия ОП
	aud_created_ts timestamp NULL,
	aud_created_by varchar(100) NULL,
	aud_last_updated_ts timestamp NULL,
	aud_last_updated_by varchar(100) NULL,
	""version"" int4 NULL,
	date_created timestamp NULL,
	plans_count int4 NOT NULL DEFAULT 0, -- Число планов ОП
	CONSTRAINT pk_curriculum PRIMARY KEY (id)
);";
    
    private const string CsPropsCode = @"
        /// <summary> ПК </summary>
        public string Id { get; set; } = default!;

        /// <summary> Код специальности </summary>
        [MaxLength(1000)]
        [Required]
        public string SpecialityCode { get; set; } = default!;

        /// <summary> ИД профиля </summary>
        public string ProfileId { get; set; } = default!;

        /// <summary> Наименование профиля </summary>
        public string Profile { get; set; } = default!;

        /// <summary> ИД плана реализации ОП </summary>
        public string CurriculumPlanId { get; set; } = default!;

        /// <summary> Наименование плана реализации ОП </summary>
        public string CurriculumPlan { get; set; } = default!;

        /// <summary> ИД версии плана реализации ОП </summary>
        public string CurriculumPlanVersionId { get; set; } = default!;

        /// <summary> Номер версии плана реализации ОП </summary>
        public string [] CurriculumPlanVersion { get; set; } = default!;

        public Dictionary<string, object> AdditionalProperties { get; set; }

        /// <summary> Активный? </summary>
        public List<List<bool>> Active { get; set; }";

    private const string CsEnumCode = @"        /// <summary> Необходимость оплаты проверки ДЗ </summary>
        public enum PaymentRequirement
        {
            /// <summary> Бесплатная проверка </summary>
            Free,

            /// <summary> Необходимо оплатить </summary>
            NeedToPay,

            /// <summary> Работа оплачена </summary>
            Payed
        }";
    
    #endregion

    [Fact]
    public async Task Convert_TsToCs_CorrectResult()
    {
        //Arrange

        //Act
        var result = await _converter.Convert(TypescriptCode, TypeScriptToCSharpConventions.Convertions);

        //Assert
        Approvals.Verify(result.Code);
    }
    
    [Fact]
    public async Task Convert_CsToTs_CorrectResult()
    {
        //Arrange

        //Act
        var result = await _converter.Convert(CSharpCode, CSharpToTypeScriptConventions.TypeToInterfaceConventions);

        //Assert
        Approvals.Verify(result.Code);
    }
    
    [Fact]
    public async Task Convert_CsToTs2_CorrectResult()
    {
        //Arrange

        //Act
        var result = await _converter.Convert(CSharpCode2, CSharpToTypeScriptConventions.TypeToInterfaceConventions);

        //Assert
        Approvals.Verify(result.Code);
    }
    
    [Fact]
    public async Task Convert_CsEnumToTs_CorrectResult()
    {
        //Arrange

        //Act
        var result = await _converter.Convert(CsEnumCode, CSharpToTypeScriptConventions.TypeToInterfaceConventions);

        //Assert
        Approvals.Verify(result.Code);
    }
    
    [Fact]
    public async Task Convert_CsParamsToTs_CorrectResult()
    {
        //Arrange

        //Act
        var result = await _converter.Convert(CSharpParams, CSharpToTypeScriptConventions.ParamsConventions);

        //Assert
        Approvals.Verify(result.Code);
    }
    
    [Fact]
    public async Task Convert_DdlToCs_CorrectResult()
    {
        //Arrange

        //Act
        var result = await _converter.Convert(DdlCode, DdlToCSharpConventions.Conventions);

        //Assert
        Approvals.Verify(result.Code);
    }
    
    [Fact]
    public async Task Convert_CsInit_CorrectResult()
    {
        //Arrange

        //Act
        var result = await _converter.Convert(CsPropsCode, CSharpInitConventions.Conventions);

        //Assert
        Approvals.Verify(result.Code);
    }
}