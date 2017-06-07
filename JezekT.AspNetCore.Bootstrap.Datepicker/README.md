# Datepicker Tag Helper

ASP .NET Core Tag Helper for datepicker. Uses [Bootstrap 3 Datepicker v4](https://eonasdan.github.io/bootstrap-datetimepicker).

## Getting Started

### Prerequisites

Make Tag Helper available to the view by adding following line to Views/_ViewImports.cshtml.

```
@addTagHelper *, JezekT.AspNetCore.Bootstrap.Datepicker
```

### Installing

1. add Tag Helper to the view

```html
<form asp-action="Create">
    <div class="form-horizontal">
        <div class="form-group">
            <label asp-for="Expiration" class="col-md-2 control-label"></label>
            <div class="col-md-4">
                <datepicker datetime-property="Expiration" format="L" class="form-control"></datepicker>
                <span asp-validation-for="Expiration" class="text-danger"></span>
            </div>
        </div>
        <div class="form-group">
            <div class="col-md-offset-2 col-md-4">
                <input type="submit" Value="Create" class="btn btn-default"/>
            </div>
        </div>
    </div>
</form>
```

>*datetime-property* - use any DateTime property from your model;

>*format* - use [format](https://eonasdan.github.io/bootstrap-datetimepicker/#custom-formats) string;

2. add CDN Tag Helper to the view

```html
<datepicker-jscss></datepicker-jscss>
```

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

